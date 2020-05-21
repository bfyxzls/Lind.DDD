using Lind.DDD.Events;
using Lind.DDD.Logger;
using Lind.DDD.Web.EventHandlers;
using Lind.DDD.Web.Events;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Lind.DDD.Web
{
    // 注意: 有关启用 IIS6 或 IIS7 经典模式的说明，
    // 请访问 http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            //注册所有事件
           // Lind.DDD.Events.EventBus.Instance.SubscribeAll();
        }

        protected void Application_Error()
        {
            Exception ex = this.Context.Server.GetLastError();
            if (ex != null)
            {
                string param = string.Empty;
                foreach (var item in Request.QueryString.AllKeys)
                {
                    param += item + "=" + Request.QueryString[item] + "；";
                }
                foreach (var item in Request.Form.AllKeys)
                {
                    param += item + "=" + Request.Form[item] + "；";
                }
                LoggerFactory.Instance.Logger_Info(string.Format("时间:{0}\r\n错误描述：{1}\r\n请求地址：{2}\r\n异常方法：{3}\r\n参数：{4}\r\n详细:{5}\r\n", DateTime.Now, ex.Message, HttpContext.Current.Request.Url, ex.TargetSite.ToString(), param, ex.StackTrace));
            }
        }

    }
}