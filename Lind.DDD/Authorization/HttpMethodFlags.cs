using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Authorization
{
    /// <summary>
    /// Http请求方式，支持位运算
    /// </summary>
    [Flags]
    public enum HttpMethodFlags
    {
        /// <summary>
        /// Get请求
        /// </summary>
        GET = 1,
        /// <summary>
        /// Post请求
        /// </summary>
        POST = 2,
        /// <summary>
        /// Put请求
        /// </summary>
        PUT = 4,
        /// <summary>
        /// Delete请求
        /// </summary>
        DELETE = 8,
    }
}
