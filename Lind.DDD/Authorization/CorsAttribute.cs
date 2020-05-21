using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lind.DDD.Authorization
{
    /// <summary>
    /// MVC模式下跨域访问
    /// </summary>
    public class MvcCorsAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("Access-Control-Allow-Origin", "*");
            headers.Add("Access-Control-Allow-Methods", "*");
            foreach (var item in headers.Keys)
            {
                filterContext.RequestContext.HttpContext.Response.Headers.Add(item, headers[item]);
            }

            base.OnActionExecuting(filterContext);
        }
    }
    /// <summary>
    /// 跨域资源共享特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class CorsAttribute : Attribute
    {
        public Uri[] AllowOrigins { get; private set; }
        public string ErrorMessage { get; private set; }
        public CorsAttribute(params string[] allowOrigins)
        {
            this.AllowOrigins = (allowOrigins ?? new string[0]).Select(origin => new Uri(origin)).ToArray();
        }
        public bool TryEvaluate(HttpRequestMessage request, out IDictionary<string, string> headers)
        {
            headers = null;
            string origin = null;
            try
            {
                origin = request.Headers.GetValues("Origin").FirstOrDefault();
            }
            catch (Exception)
            {
                this.ErrorMessage = "Cross-origin request denied";
                return false;
            }
            Uri originUri = new Uri(origin);
            if (this.AllowOrigins.Contains(originUri))
            {
                headers = this.GenerateResponseHeaders(request);
                return true;
            }

            this.ErrorMessage = "Cross-origin request denied";
            return false;
        }

        private IDictionary<string, string> GenerateResponseHeaders(HttpRequestMessage request)
        {

            //设置响应头"Access-Control-Allow-Methods"

            string origin = request.Headers.GetValues("Origin").First();

            Dictionary<string, string> headers = new Dictionary<string, string>();

            headers.Add("Access-Control-Allow-Origin", origin);

            if (request.IsPreflightRequest())
            {
                //设置响应头"Access-Control-Request-Headers"
                //和"Access-Control-Allow-Headers"
                headers.Add("Access-Control-Allow-Methods", "*");

                string requestHeaders = request.Headers.GetValues("Access-Control-Request-Headers").FirstOrDefault();

                if (!string.IsNullOrEmpty(requestHeaders))
                {
                    headers.Add("Access-Control-Allow-Headers", requestHeaders);
                }
            }
            return headers;
        }
    }

    /// <summary>
    /// HttpRequestMessage扩展方法
    /// </summary>
    public static class HttpRequestMessageExtensions
    {
        public static bool IsPreflightRequest(this HttpRequestMessage request)
        {
            return request.Method == HttpMethod.Options
                && request.Headers.GetValues("Origin").Any()
                && request.Headers.GetValues("Access-Control-Request-Method").Any();
        }
    }
}
