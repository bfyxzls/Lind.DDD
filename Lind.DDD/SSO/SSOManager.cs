using Lind.DDD.ConfigConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lind.DDD.SSO
{

    /// <summary>
    /// 缓存管理
    /// 将用户凭证、令牌的关系数据存放于Cache中
    /// </summary>
    public class SSOManager
    {
        static ISSOProvider _instance;
        static object lockObj = new object();
        /// <summary>
        /// SSO资源提供者
        /// </summary>
        public static ISSOProvider Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (_instance == null)
                    {
                        lock (lockObj)
                        {
                            switch (ConfigManager.Config.SSO.Provider.ToLower())
                            {
                                case "cache":
                                    _instance = new Implements.CacheProvider();
                                    break;
                                case "redis":
                                    _instance = new Implements.RedisProvider();
                                    break;
                                default:
                                    throw new ArgumentException("不能识别的SSO提供者!");
                            }
                        }
                    }
                }
                return _instance;
            }
        }

        #region Public Methods
        /// <summary>
        ///  登陆SSO,这通常是由服务平台提供的统一入口
        /// </summary>
        /// <param name="userId">用户ID</param>
        /// <param name="userName">用户名</param>
        /// <param name="backUrl">回调路径</param>
        /// <returns></returns>
        public static RedirectToRouteResult LoginSSO(string userId, string userName, string backUrl)
        {
            return new RedirectToRouteResult("Default", new RouteValueDictionary
            { 
               { "Action","DoCertification" },
               { "Controller", "SSOCore"}, 
               { "userId", userId} ,
               { "userName", userName},
               { "BackUrl", backUrl} 
             });
        }
        /// <summary>
        /// 登出SSO,登出操作,业务平台会调用这个方法
        /// </summary>
        public static void ExitSSO()
        {
            var Request = System.Web.HttpContext.Current.Request;
            //清空主站凭证
            if (Request.QueryString["Token"] == null)
            {
                System.Web.HttpContext.Current.Response.Redirect(Lind.DDD.ConfigConstants.ConfigManager.Config.SSO.Domain + "/SSOCore/GetTokenUrl?BackURL=" + System.Web.HttpUtility.UrlEncode(Request.Url.AbsoluteUri + "?Token=$Token$"));
            }
            else
            {
                if (Request.QueryString["Token"] != "$Token$")
                {
                    string token = Request.QueryString["Token"];
                    string url = Lind.DDD.ConfigConstants.ConfigManager.Config.SSO.Domain + "/SSOCore/ClearToken";
                    using (var http = new HttpClient())
                    {
                        //使用FormUrlEncodedContent做HttpContent
                        var content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "tokenValue", token } });
                        http.PostAsync(url, content).Wait();
                    }
                    //清空本地凭证
                    System.Web.HttpContext.Current.Session.Abandon();
                }
            }
        }
        #endregion
    }
}
