using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.WebPay
{
    /// <summary>
    /// http请求方式
    /// </summary>
    public enum HttpMethod
    {
        /// <summary>
        ///　get方式，以request.querystring传递参数，如URL跳转
        /// </summary>
        Get,
        /// <summary>
        /// post方式，以request.form传递参数，如表单提交
        /// </summary>
        Post,

    }
}
