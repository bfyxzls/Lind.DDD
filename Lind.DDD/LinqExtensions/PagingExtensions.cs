using Lind.DDD.Paging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Linq
{
    /// <summary>
    /// IQueryable<T>的扩展方法
    /// 反回指定人IQueryable结果集
    /// </summary>
    public static class ExtendIQueryable
    {
        /// <summary>
        /// 分页结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linq"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> linq, int pageIndex, int pageSize)
        {
            return new PagedList<T>(linq, pageIndex, pageSize);
        }
        /// <summary>
        /// 分页结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linq"></param>
        /// <param name="pp"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList<T>(this IQueryable<T> linq, PageParameters pp)
        {
            return new PagedList<T>(linq, pp.PageIndex, pp.PageSize);
        }
        /// <summary>
        /// 分页结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linq"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList<T>(this IOrderedEnumerable<T> linq, int pageIndex, int pageSize)
        {
            return new PagedList<T>(linq.AsQueryable(), pageIndex, pageSize);
        }
        /// <summary>
        /// 分页结果集
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="linq"></param>
        /// <param name="pp"></param>
        /// <returns></returns>
        public static PagedList<T> ToPagedList<T>(this IOrderedEnumerable<T> linq, PageParameters pp)
        {
            return new PagedList<T>(linq.AsQueryable(), pp.PageIndex, pp.PageSize);
        }
    }
}
