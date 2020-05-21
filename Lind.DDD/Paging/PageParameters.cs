using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Paging
{
    /// <summary>
    /// 通用分页参数 结构
    /// </summary>
    public struct PageParameters
    {

        /// <summary>
        /// 页面大小
        /// </summary>
        public int PageSize { get; private set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 通用分页参数结构 构造函数
        /// </summary>
        /// <param name="pageIndex">页码</param>
        /// <param name="pageSize">页面大小</param>
        public PageParameters(int pageIndex, int pageSize)
            : this()
        {
            this.PageIndex = pageIndex;
            this.PageSize = pageSize;
        }

    }

}
