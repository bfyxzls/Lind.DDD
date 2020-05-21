using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 对IQueryable`1集合进行分批处理
    /// </summary>
    public class QueryablePagingAction
    {
        /// <summary>
        /// 分页处理集合，method是具体要处理的事件，默认每批处理5000条数据
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="linq"></param>
        /// <param name="method"></param>
        public static void Run<TEntity>(IQueryable<TEntity> linq, Action<IEnumerable<TEntity>> method)
        {
            Run(linq, 5000, method);
        }
        /// <summary>
        /// 分页处理集合，method是具体要处理的事件
        /// </summary>
        /// <typeparam name="TEntity">类型</typeparam>
        /// <param name="linq">可查询的集合</param>
        /// <param name="dataPageSize">每次处理的大小</param>
        /// <param name="method">回调方法</param>
        public static void Run<TEntity>(IQueryable<TEntity> linq, int dataPageSize, Action<IEnumerable<TEntity>> method)
        {
            int _dataTotalCount = 0;
            int _dataTotalPages = 0;
            if (linq != null && linq.Any())
            {
                _dataTotalCount = linq.Count();
                _dataTotalPages = linq.Count() / dataPageSize;
                if (_dataTotalCount % dataPageSize > 0)
                    _dataTotalPages += 1;
                for (int pageIndex = 1; pageIndex <= _dataTotalPages; pageIndex++)
                {
                    var currentItems = linq.Skip((pageIndex - 1) * dataPageSize).Take(dataPageSize).ToList();
                    method(currentItems);
                }
            }
        }

    }
}
