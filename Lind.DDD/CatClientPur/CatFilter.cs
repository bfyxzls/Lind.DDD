using PureCat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CatClientPur
{
    /// <summary>
    /// Cat拦截器，主要拦截Http请求
    /// </summary>
    public class CatFilter : System.Web.Mvc.ActionFilterAttribute
    {
        static string catProjectname = System.Configuration.ConfigurationManager.AppSettings["CatProjectname"] ?? "Lind";
        /// <summary>
        /// 请求来到时
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuting(System.Web.Mvc.ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        /// <summary>
        /// 请求结束时
        /// 调用次序：A->B->C->c->b->a,从c开始执行，把context结果在响应头里依据向回传
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnActionExecuted(System.Web.Mvc.ActionExecutedContext filterContext)
        {


            var context = PureCat.CatClient.GetCatContextFromServer();
            if (context != null)
            {

                context = PureCat.CatClient.DoTransaction(catProjectname, filterContext.HttpContext.Request.Url.AbsoluteUri, () =>
                {
                    PureCat.CatClient.LogRemoteCallServer(context);

                    PureCat.CatClient.LogEvent(filterContext.HttpContext.Request.Url.AbsoluteUri, "Action  Finish...");

                    if (filterContext.Exception != null)
                    {
                        PureCat.CatClient.LogError(filterContext.Exception);
                    }
                });

                PureCat.CatClient.SetCatContextToResponseHeader(filterContext.HttpContext.Response, context);

            }

            base.OnActionExecuted(filterContext);

        }
    }
}
