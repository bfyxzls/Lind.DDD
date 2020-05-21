using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Filters;
using System.Web.Mvc;

namespace Lind.DDD.Filters
{
    /// <summary>
    /// API全局异常信息收集器
    /// 使用:WebApiConfig/Register/  config.Filters.Add(new Lind.DDD.Exceptions.ExceptionErrorLoggerAttribute());
    /// </summary>
    public class ExceptionErrorLoggerAttribute : ExceptionFilterAttribute
    {
        /// <summary>
        /// 出现异常时进入此方法
        /// </summary>
        /// <param name="actionExecutedContext"></param>
        public override void OnException(HttpActionExecutedContext actionExecutedContext)
        {

            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Error(actionExecutedContext.Exception);

            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("\r\n当前请求出现异常,时间:"
                + DateTime.Now
                + "\r\n异常URL:"
                + actionExecutedContext.Request.RequestUri.AbsoluteUri
                + "\r\n异常信息:" + actionExecutedContext.Exception.Message);
            base.OnException(actionExecutedContext);
        }

    }
    public class MvcExceptionErrorLoggerAttribute : HandleErrorAttribute
    {
        #region IExceptionFilter 成员

        public override void OnException(ExceptionContext filterContext)
        {
            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Error(filterContext.Exception);

            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("\r\n当前请求出现异常,时间:"
                + DateTime.Now
                + "\r\n异常URL:"
                + filterContext.RequestContext.HttpContext.Request.Url.AbsoluteUri
                + "\r\n异常信息:" + filterContext.Exception.Message
                + "\r\n堆栈信息:" + filterContext.Exception.StackTrace);
            base.OnException(filterContext);
            filterContext.RequestContext.HttpContext.Response.Write(
                "<form id='errForm' method='post' action='/AdminCommon/Error'><input type='hidden' name='msgErr' value='" + filterContext.Exception.StackTrace + "'></form><script>document.forms['errForm'].submit();</script>"
                );


        }

        #endregion
    }
}
