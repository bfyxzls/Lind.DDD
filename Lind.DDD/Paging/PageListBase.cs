using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using Lind.DDD.Paging;

namespace Lind.DDD.Paging
{
    /// <summary>
    /// 分页中包含的公开的属性
    /// </summary>
    public interface IPagedList
    {
        /// <summary>
        /// 记录数
        /// </summary>
        int TotalCount { get; set; }
        /// <summary>
        /// 页数
        /// </summary>
        int TotalPages { get; set; }
        /// <summary>
        /// 当前页
        /// </summary>
        int PageIndex { get; set; }
        /// <summary>
        /// 页面大小
        /// </summary>
        int PageSize { get; set; }
        /// <summary>
        /// 是否上一页
        /// </summary>
        bool IsPreviousPage { get; }
        /// <summary>
        /// 是否下一页
        /// </summary>
        bool IsNextPage { get; }
    }
    /// <summary>
    /// 分页功能基数
    /// </summary>
    public abstract class PageListBase<T> : 
        List<T>, 
        IEnumerable<T>, 
        IPagedList //繼承List<T>可以使用它的内部方法，如AddRange,Skip,Take等
    {
        /// <summary>
        /// 初始化分页
        /// </summary>
        public PageListBase()
        {
            this.AddParameters = new NameValueCollection();
            this.PageSize = 10;
        }

        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// 总记录数
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// 当前页
        /// </summary>
        public int PageIndex { get; set; }

        /// <summary>
        /// 每页显示多少条记录
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 是否有上一页
        /// </summary>
        public bool IsPreviousPage { get { return (PageIndex > 0); } }

        /// <summary>
        /// 是否有下一页
        /// </summary>
        public bool IsNextPage { get { return (PageIndex * PageSize) <= TotalCount; } }

        /// <summary>
        /// 分页参数
        /// </summary>
        public NameValueCollection AddParameters { get; set; }
    }

}

