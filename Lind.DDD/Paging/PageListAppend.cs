using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.Specialized;

namespace Lind.DDD.Paging
{

    /// <summary>
    /// 以追加的方式，显示内容，即从第1页开始显示N条记录
    /// </summary>
    /// <typeparam name="T"></typeparam>
     public class PageListAppend<T> : PageListBase<T>
    {
        /// <summary>
        /// 数据源为IQueryable的范型
        /// </summary>
        /// <param name="source">数据源</param>
        /// <param name="index">当前页</param>
        /// <param name="pageSize">每页显示多少条记录</param>
        public PageListAppend(IQueryable<T> source, int index, int pageSize)
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
                this.AddRange(source.Skip(0).Take(index * pageSize).ToList()); //Skip是跳到第几页，Take返回多少条
            }
        }
    }



}