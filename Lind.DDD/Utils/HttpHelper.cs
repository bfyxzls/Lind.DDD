using Lind.DDD.Authorization.Api;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// Http相关操作
    /// </summary>
    public class HttpHelper
    {

        /// <summary>
        /// 加工当前Uri
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="nv"></param>
        /// <returns></returns>
        private static string GeneratorUri(string requestUri, NameValueCollection nv)
        {
            if (nv != null)
            {
                if (requestUri.IndexOf("?") > -1)
                {
                    requestUri += "&" + nv.ToUrl();
                }
                else
                {
                    requestUri += "?" + nv.ToUrl();
                }
            }
            return requestUri;
        }
        /// <summary>
        /// 返回一个httpHandler
        /// </summary>
        /// <param name="uri">要发送的url</param>
        /// <param name="isSendSessionId">是否要同时发sessionId</param>
        /// <returns></returns>
        private static HttpClientHandler GetHttpHandler(string uri, bool isSendSessionId)
        {
            HttpClientHandler handler = new HttpClientHandler() { AutomaticDecompression = System.Net.DecompressionMethods.GZip };
            if (isSendSessionId)
            {
                handler.CookieContainer = new CookieContainer();
                handler.CookieContainer.Add(new Uri(uri), new Cookie("ASP.NET_SessionId", System.Web.HttpContext.Current.Session.SessionID)); // Adding a Cookie
            }
            return handler;
        }

        #region Get方法
        public static HttpResponseMessage Get(string requestUri)
        {
            return Get(requestUri, null, 60, false);
        }
        public static HttpResponseMessage Get(string requestUri, NameValueCollection nv)
        {
            return Get(requestUri, nv, 60, false);
        }
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="nv"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static HttpResponseMessage Get(
         string requestUri,
         NameValueCollection nv,
         int timeOut)
        {
            return Get(requestUri, nv, timeOut, false);
        }
        /// <summary>
        /// GET请求
        /// </summary>
        /// <param name="requestUri">服务地址</param>
        /// <param name="nv">参数键值</param>
        /// <param name="timeOut">超时</param>
        /// <param name="isSendSessionId">是否将自己sessionid传到requestUri服务上</param>
        /// <returns></returns>
        public static HttpResponseMessage Get(
            string requestUri,
            NameValueCollection nv,
            int timeOut,
            bool isSendSessionId)
        {
            using (var http = new HttpClient(GetHttpHandler(requestUri, isSendSessionId)))
            {    //超时
                http.Timeout = new TimeSpan(0, 0, timeOut);
                HttpResponseMessage response;
                try
                {
                    response = http.GetAsync(GeneratorUri(requestUri, ApiValidateHelper.GenerateCipherText(nv))).Result;
                }
                catch (Exception ex)
                {
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout) { Content = new StringContent("请求超时") };
                    Logger.LoggerFactory.Instance.Logger_Error(ex);
                }

                return response;
            }
        }

        #endregion

        #region Post方法
        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="requestUri">请求地址</param>
        /// <param name="nv">参数键值</param>
        /// <param name="timeOut">超时</param>
        /// <param name="isSendSessionId">是否发送sesionid</param>
        /// <returns>响应流</returns>
        public static HttpResponseMessage Post(
            string requestUri,
            NameValueCollection nv,
            int timeOut,
            bool isSendSessionId)
        {
            using (var http = new HttpClient(GetHttpHandler(requestUri, isSendSessionId)))
            {
                http.Timeout = new TimeSpan(0, 0, timeOut);
                HttpResponseMessage response;
                try
                {
                    var body = new Dictionary<string, string>();

                    foreach (string item in nv.Keys)
                    {
                        body.Add(item, nv[item]);
                    }

                    response = http.PostAsync(GeneratorUri(
                        requestUri,
                        ApiValidateHelper.GenerateCipherText(nv, true)),
                     new FormUrlEncodedContent(body)).Result; //StringContent这种方式Request.Form无法拿到值

                    //response = http.PostAsync(GeneratorUri(
                    //     requestUri,
                    //     ApiValidateHelper.GenerateCipherText(nv, true)),
                    //  new StringContent(body.ToJson(), Encoding.UTF8, "application/json")).Result;
                }
                catch (Exception ex)
                {
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout)
                    {
                        Content = new StringContent("请求超时")
                    };
                    Logger.LoggerFactory.Instance.Logger_Error(ex);
                }

                return response;
            }
        }

        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="nv"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post(
        string requestUri,
        NameValueCollection nv,
        int timeOut)
        {
            return Post(requestUri, nv, timeOut, false);
        }
        public static HttpResponseMessage Post(string requestUri, NameValueCollection nv)
        {
            return Post(requestUri, nv, 60, false);
        }
        #endregion

        #region Post<T>方法
        /// <summary>
        /// POST请求
        /// </summary>
        /// <param name="requestUri">请求地址</param>
        /// <param name="entity">参数实体</param>
        /// <param name="timeOut">超时</param>
        /// <param name="isSendSessionId">是否发送sesionid</param>
        /// <returns>响应流</returns>
        public static HttpResponseMessage Post<T>(
            string requestUri,
            T entity,
            int timeOut,
            bool isSendSessionId)
        {
            return Post(requestUri, entity.ToNameValueCollection(), timeOut, isSendSessionId);
        }

        /// <summary>
        ///  POST请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="entity"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static HttpResponseMessage Post<T>(
          string requestUri,
          T entity,
          int timeOut)
        {
            return Post<T>(requestUri, entity, timeOut, false);
        }
        public static HttpResponseMessage Post<T>(
        string requestUri,
        T entity)
        {
            return Post<T>(requestUri, entity, 60, false);
        }
        #endregion

        #region Put
        /// <summary>
        ///  PUT请求
        /// </summary>
        /// <param name="requestUri">请求地址</param>
        /// <param name="nv">参数键值</param>
        /// <param name="timeOut">超时</param>
        /// <returns>响应流</returns>
        public static HttpResponseMessage Put(
            string requestUri,
            NameValueCollection nv,
            int timeOut,
            bool isSendSessionId)
        {
            using (var http = new HttpClient(GetHttpHandler(requestUri, isSendSessionId)))
            {
                http.Timeout = new TimeSpan(0, 0, timeOut);
                HttpResponseMessage response;
                try
                {
                    var body = new Dictionary<string, string>();
                    foreach (string item in nv.Keys)
                    {
                        body.Add(item, nv[item]);
                    }
                    response = http.PutAsync(GeneratorUri(
                       requestUri,
                       ApiValidateHelper.GenerateCipherText(nv, true)),
                       new FormUrlEncodedContent(body)).Result;
                }
                catch (Exception ex)
                {
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout) { Content = new StringContent("请求超时") };
                    Logger.LoggerFactory.Instance.Logger_Error(ex);
                }

                return response;
            }
        }
        /// <summary>
        /// PUT请求
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="nv"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static HttpResponseMessage Put(
         string requestUri,
         NameValueCollection nv,
         int timeOut)
        {
            return Put(requestUri, nv, timeOut, false);
        }

        public static HttpResponseMessage Put(
       string requestUri,
       NameValueCollection nv)
        {
            return Put(requestUri, nv, 60, false);
        }
        #endregion

        #region Put<T>方法
        /// <summary>
        /// PUT请求
        /// </summary>
        /// <param name="requestUri">请求对象</param>
        /// <param name="entity">实体</param>
        /// <param name="timeOut">超时</param>
        /// <param name="isSendSessionId">是否发sessionid</param>
        /// <returns></returns>
        public static HttpResponseMessage Put<T>(
            string requestUri,
            T entity,
            int timeOut,
            bool isSendSessionId)
        {
            return Put(requestUri, entity.ToNameValueCollection(), timeOut, isSendSessionId);
        }
        /// <summary>
        /// PUT请求
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="requestUri"></param>
        /// <param name="entity"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static HttpResponseMessage Put<T>(
          string requestUri,
          T entity,
          int timeOut)
        {
            return Put<T>(requestUri, entity, timeOut, false);
        }

        public static HttpResponseMessage Put<T>(
          string requestUri,
          T entity)
        {
            return Put<T>(requestUri, entity, 60, false);
        }
        #endregion

        #region Delete方法
        /// <summary>
        /// DELETE请求
        /// </summary>
        /// <param name="requestUri">源地址</param>
        /// <param name="content">请求主体</param>
        /// <param name="nv">追加到URL上的参数</param>
        /// <param name="isSendSessionId">是否发sessionid</param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(
            string requestUri,
            NameValueCollection nv,
            int timeOut,
           bool isSendSessionId)
        {

            using (var http = new HttpClient(GetHttpHandler(requestUri, isSendSessionId)))
            {
                http.Timeout = new TimeSpan(0, 0, timeOut);
                HttpResponseMessage response;
                try
                {
                    response = http.DeleteAsync(GeneratorUri(requestUri, ApiValidateHelper.GenerateCipherText(nv))).Result;
                }
                catch (Exception ex)
                {
                    response = new HttpResponseMessage(System.Net.HttpStatusCode.RequestTimeout) { Content = new StringContent("请求超时") };
                    Logger.LoggerFactory.Instance.Logger_Error(ex);

                }
                return response;
            }
        }
        /// <summary>
        ///  DELETE请求
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="nv"></param>
        /// <param name="timeOut"></param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(
           string requestUri,
           NameValueCollection nv,
           int timeOut)
        {
            return Delete(requestUri, nv, timeOut, false);
        }
        /// <summary>
        /// DELETE请求
        /// </summary>
        /// <param name="requestUri"></param>
        /// <param name="nv"></param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(
         string requestUri,
         NameValueCollection nv)
        {
            return Delete(requestUri, nv, 60, false);
        }
        /// <summary>
        /// DELETE请求
        /// </summary>
        /// <param name="requestUri"></param>
        /// <returns></returns>
        public static HttpResponseMessage Delete(
          string requestUri)
        {
            return Delete(requestUri, null, 60, false);
        }
        #endregion

    }
}
