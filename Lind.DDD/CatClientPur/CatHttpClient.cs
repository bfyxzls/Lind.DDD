using PureCat;
using PureCat.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CatClientPur
{
    /// <summary>
    /// cat中使用的HttpClient
    /// </summary>
    public class CatHttpClient
    {
        /// <summary>
        /// 返回当前Cat上下文
        /// </summary>
        /// <returns></returns>
        static CatContext GetCurrentContext(string message, bool isCat)
        {

            string currentUrl = System.Web.HttpContext.Current.Request.Url.AbsoluteUri;
            var context = PureCat.CatClient.GetCatContextFromServer();
            if (isCat)
            {

                if (context == null)
                {
                    context = PureCat.CatClient.DoTransaction("xuexiba", currentUrl, () =>
                    {
                        PureCat.CatClient.LogEvent("xuexiba", message, "0", currentUrl);
                    });

                }
                else
                {

                    context = PureCat.CatClient.DoTransaction("xuexiba", currentUrl, () =>
                    {
                        PureCat.CatClient.LogRemoteCallServer(context);
                        PureCat.CatClient.LogEvent("xuexiba", message, "0", currentUrl);
                    });
                }
            }
            else
            {
                if (context == null)
                {
                    context = PureCat.CatClient.LogRemoteCallClient(Guid.NewGuid().ToString());
                }

            }
            return context;
        }
        /// <summary>
        /// Post数据
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post(string requestUri, bool isCat, HttpContent content)
        {
            var handler = new HttpClientHandler() { };
            var response = new HttpResponseMessage();
            using (var http = new HttpClient(handler))
            {
                http.Timeout = new TimeSpan(0, 0, 60);
                try
                {

                    PureCat.CatClient.SetCatContextToRequestHeader(http, GetCurrentContext("Post Request Sent...", isCat));
                    response = http.PostAsync(requestUri, content).Result;
                    PureCat.CatClient.SetCatContextToRequestHeader(response);
                    return response;
                }
                catch (Exception)
                {
                    try
                    {
                        response = http.PostAsync(requestUri, content).Result;
                    }
                    catch (Exception ex1)
                    {
                        Logger.LoggerFactory.Instance.Logger_Warn("catPost1" + ex1.Message);
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout) { Content = new StringContent("请求超时") };
                    }
                    return response;
                }

            }
        }
        public static HttpResponseMessage Post(string requestUri, HttpContent content)
        {
            return Post(requestUri, false, content);
        }
        /// <summary>
        /// Get数据
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static HttpResponseMessage Get(string requestUri)
        {
            return Get(requestUri, false);
        }
        /// <summary>
        /// Get数据
        /// </summary>
        /// <param name="requestUri">调用的uri</param>
        /// <param name="isCat">是否写cat</param>
        /// <returns></returns>
        public static HttpResponseMessage Get(string requestUri, bool isCat)
        {
            var handler = new HttpClientHandler() { };
            using (var http = new HttpClient(handler))
            {
                http.Timeout = new TimeSpan(0, 0, 60);
                var response = new HttpResponseMessage();
                try
                {

                    PureCat.CatClient.SetCatContextToRequestHeader(http, GetCurrentContext("Get Request Sent...", isCat));//设置接口api的头，发送
                    response = http.GetAsync(requestUri).Result;
                    PureCat.CatClient.SetCatContextToRequestHeader(response);
                    return response;
                }

                catch (Exception)
                {
                    try
                    {
                        response = http.GetAsync(requestUri).Result;
                    }
                    catch (Exception)
                    {
                        response = new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout) { Content = new StringContent("请求超时") };
                    }

                    return response;
                }
            }

        }


    }
}
