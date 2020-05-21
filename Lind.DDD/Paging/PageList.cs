using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace Lind.DDD.Paging
{

    /// <summary>
    /// 分页通用类(传统分页)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PagedList<T> : PageListBase<T>
    {
        public PagedList()
            : this(null, 0, 0)
        {
        }
        /// <summary>
        /// 数据源为IQueryable的范型
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="index">当前页</param>
        /// <param name="pageSize">每页显示多少条记录</param>
        public PagedList(IQueryable<T> source, int index, int pageSize)
        {
            if (source != null) //判断传过来的实体集是否为空
            {
                int total = source.Count();
                this.TotalCount = total;
                this.TotalPages = total / pageSize;

                if (total % pageSize > 0)
                    TotalPages++;

                this.PageSize = pageSize;
                if (index > this.TotalPages)
                {
                    index = this.TotalPages;
                }
                if (index < 1)
                {
                    index = 1;
                }
                this.PageIndex = index;
                this.AddRange(source.Skip((index - 1) * pageSize).Take(pageSize).ToList()); //Skip是跳到第几页，Take返回多少条
            }
        }
    }
}