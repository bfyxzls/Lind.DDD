using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Authorization.Api
{
    /// <summary>
    /// 跨域资源共享特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CorsFilter : System.Web.Http.Filters.ActionFilterAttribute
    {
        public string[] AllowOrigins { get; private set; }
       
        public CorsFilter(params string[] allowOrigins)
        {
            this.AllowOrigins = allowOrigins;
        }

        public override void OnActionExecuting(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            #region 例外
            bool skipAuthorization = actionContext.ControllerContext.ControllerDescriptor.ControllerType.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
                actionContext.ControllerContext.ControllerDescriptor.ControllerType.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            if (skipAuthorization)
                return;
            #endregion

            var context = (HttpContextBase)actionContext.Request.Properties["MS_HttpContext"];//获取传统context
            var request = context.Request;//定义传统request对象
            var currentUri = request.Url.Host;

            if (AllowOrigins.Contains("*"))
                return;
            if (!AllowOrigins.Contains(currentUri))
            {
                actionContext.Response = new HttpResponseMessage(HttpStatusCode.Forbidden) { Content = new StringContent("未经授权的域名", Encoding.GetEncoding("UTF-8")) };
                return;
            }

            base.OnActionExecuting(actionContext);
        }
    }

}
