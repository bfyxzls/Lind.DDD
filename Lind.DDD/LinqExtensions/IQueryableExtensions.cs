//===================================================================================
// Microsoft Developer & Platform Evangelism
//=================================================================================== 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
//===================================================================================
// Copyright (c) Microsoft Corporation.  All Rights Reserved.
// This code is released under the terms of the MS-LPL license, 
// http://microsoftnlayerapp.codeplex.com/license
//===================================================================================
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Globalization;
using System.Data.Entity.Core.Objects;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Threading.Tasks;
using System.Text;

namespace System.Linq
{

    /// <summary>
    /// Class for IQuerable extensions methods
    /// <remarks>
    /// Include method in IQueryable ( base contract for IObjectSet ) is 
    /// intended for mock Include method in ObjectQuery{T}.
    /// Paginate solve not parametrized queries issues with skip and take L2E methods
    /// </remarks>
    /// </summary>
    public static class IQueryableExtensions
    {
        /// <summary>
        /// 按或进行位运算
        /// 作者：仓储大叔
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static int BinaryOr<TSource>(this IEnumerable<TSource> source, Func<TSource, int> selector)
        {
            int result = 0;
            foreach (var item in source)
            {
                result |= selector(item);
            }
            return result;
        }
        /// <summary>
        /// 按或进行位运算
        /// 作者：仓储大叔
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static long BinaryOr<TSource>(this IEnumerable<TSource> source, Func<TSource, long> selector)
        {
            long result = 0;
            foreach (var item in source)
            {
                result |= selector(item);
            }
            return result;
        }
        /// <summary>
        /// 将分组字符进行拼接
        /// 作者：仓储大叔
        /// </summary>
        /// <typeparam name="TSource"></typeparam>
        /// <param name="source"></param>
        /// <param name="selector"></param>
        /// <returns></returns>
        public static string StringAnd<TSource>(this IEnumerable<TSource> source, Func<TSource, string> selector)
        {
            List<string> result = new List<string>();
            foreach (var item in source)
            {
                result.Add(selector(item));
            }
            return string.Join(",", result);
        }
        #region Extension Methods

        /// <summary>
        /// Include method for IQueryable
        /// </summary>
        /// <typeparam name="TEntity">Type of elements</typeparam>
        /// <param name="queryable">Queryable object</param>
        /// <param name="path">Path to include</param>
        /// <returns>Queryable object with include path information</returns>
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> queryable, string path)
            where TEntity : class
        {


            if (String.IsNullOrEmpty(path))
                throw new ArgumentNullException("path can not empty");
            //   var query = queryable as ObjectQuery<TEntity>;//ObjectContext時用
            var query = queryable as DbQuery<TEntity>;//DbContext時用

            if (query != null)//if is a EF ObjectQuery object
                return query.Include(path);
            return null;
        }

        /// <summary>
        /// Include extension method for IQueryable
        /// </summary>
        /// <typeparam name="TEntity">Type of elements in IQueryable</typeparam>
        /// <param name="queryable">Queryable object</param>
        /// <param name="path">Expression with path to include</param>
        /// <returns>Queryable object with include path information</returns>
        public static IQueryable<TEntity> Include<TEntity>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, object>> path)
            where TEntity : class
        {
            return Include<TEntity>(queryable, System.Web.Mvc.ExpressionHelper.GetExpressionText(path));

            // return Include<TEntity>(queryable, AnalyzeExpressionPath(path));
        }


        /// <summary>
        /// Paginate query in a specific page range
        /// </summary>
        /// <typeparam name="TEntity">Typeof entity in underlying query</typeparam>
        /// <typeparam name="S">Typeof ordered data value</typeparam>
        /// <param name="queryable">Query to paginate</param>
        /// <param name="orderBy">Order by expression used in paginate method
        /// <remarks>
        /// At this moment Order by expression only support simple order by c=>c.CustomerCode. If you need
        /// add more complex order functionality don't use this extension method
        /// </remarks>
        /// </param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageCount">Page count</param>
        /// <param name="ascending">order direction</param>
        /// <returns>A paged queryable</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public static IQueryable<TEntity> Paginate<TEntity, S>(this IQueryable<TEntity> queryable, Expression<Func<TEntity, S>> orderBy, int pageIndex, int pageCount, bool ascending)
            where TEntity : class
        {
            ObjectQuery<TEntity> query = queryable as ObjectQuery<TEntity>;

            if (query != null)
            {
                //this paginate method use ESQL for solve problems with Parametrized queries
                //in L2E and Skip/Take methods

                string orderPath = AnalyzeExpressionPath<TEntity, S>(orderBy);

                return query.Skip(string.Format(CultureInfo.InvariantCulture, "it.{0} {1}", orderPath, (ascending) ? "asc" : "desc"), "@skip", new ObjectParameter("skip", (pageIndex) * pageCount))
                            .Top("@limit", new ObjectParameter("limit", pageCount));

            }
            else // for In-Memory object set
                return queryable.OrderBy(orderBy).Skip((pageIndex * pageCount)).Take(pageCount);
        }

        /// <summary>
        /// 每次处理的记录数据
        /// </summary>
        const int DATAPAGESIZE = 10000;
        /// <summary>
        /// 对iqueryable结果每次分批ToList，防止大数量时的内存占用过高的问题
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="method"></param>
        public static void QueryablePageProcess<TEntity>(this IQueryable<TEntity> queryable, Action<IEnumerable<TEntity>> method) where TEntity : class
        {
            int DataTotalCount = 0;
            int DataTotalPages = 0;
            if (queryable != null && queryable.Count() > 0)
            {
                DataTotalCount = queryable.Count();
                DataTotalPages = queryable.Count() / DATAPAGESIZE;
                if (DataTotalCount % DATAPAGESIZE > 0)
                    DataTotalPages += 1;
                for (int pageIndex = 1; pageIndex <= DataTotalPages; pageIndex++)
                {
                    var currentItems = queryable.Skip((pageIndex - 1) * DATAPAGESIZE).Take(DATAPAGESIZE).ToList();
                    method(currentItems);
                }
            }
        }
        /// <summary>
        /// 对IEnumerable,IList,ICollection等本地结果集分批处理
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="enumerable"></param>
        /// <param name="method"></param>
        public static void EnumerablePageProcess<TEntity>(this IEnumerable<TEntity> enumerable, Action<IEnumerable<TEntity>> method) where TEntity : class
        {
            enumerable.AsQueryable().QueryablePageProcess(method);
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// 返回lambda表达示所对应的字符，骨时会出现异常
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <typeparam name="S"></typeparam>
        /// <param name="expression"></param>
        /// <returns></returns>
        static string AnalyzeExpressionPath<TEntity, S>(Expression<Func<TEntity, S>> expression)
           where TEntity : class
        {
            if (expression == (Expression<Func<TEntity, S>>)null)
                throw new ArgumentNullException("Argument error");

            MemberExpression body = expression.Body as MemberExpression;
            if (
                    (
                    (body == null)
                    ||
                    !body.Member.DeclaringType.IsAssignableFrom(typeof(TEntity))
                    )
                    ||
                    (body.Expression.NodeType != ExpressionType.Parameter))
            {
                throw new ArgumentException("Argument error");
            }
            else
                return body.Member.Name;
        }
        #endregion

        /// <summary>
        /// 并行分页处理数据，提高系统利用率，提升系统性能
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="item"></param>
        /// <param name="method"></param>
        public async static Task DataPageProcessAsync<T>(IQueryable<T> item, Action<IEnumerable<T>> method) where T : class
        {
            await Task.Run(() =>
            {
                DataPageProcess<T>(item, method);
            });
        }

        /// <summary>
        /// 在主线程上分页处理数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <param name="method"></param>
        public static void DataPageProcess<T>(IQueryable<T> item, Action<IEnumerable<T>> method) where T : class
        {
            if (item != null && item.Count() > 0)
            {
                var DataPageSize = 100;
                var DataTotalCount = item.Count();
                var DataTotalPages = item.Count() / DataPageSize;
                if (DataTotalCount % DataPageSize > 0)
                    DataTotalPages += 1;

                for (int pageIndex = 1; pageIndex <= DataTotalPages; pageIndex++)
                {
                    var currentItems = item.Skip((pageIndex - 1) * DataPageSize).Take(DataPageSize).ToList();
                    method(currentItems);
                }
            }
        }

    }
}
