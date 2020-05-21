using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lind.DDD.Manager
{
    public class RouteConfig
    {
        /// <summary>
        /// 开始的action
        /// </summary>
        static string start_Action = System.Configuration.ConfigurationManager.AppSettings["start_Action"] ?? "Index";
        /// <summary>
        /// 开始的controller
        /// </summary>
        static string start_Controller = System.Configuration.ConfigurationManager.AppSettings["start_Controller"] ?? "WebUser";
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "WebUser", action = "Index", id = UrlParameter.Optional }
            );


            routes.MapRoute(
               "LindAdmin", // 路由名称，这个只要保证在路由集合中唯一即可
               "LindAdmin/{controller}/{action}/{id}",
                new { controller = start_Controller, action = start_Action, id = UrlParameter.Optional }
            );

        }
    }

    /// <summary>
    /// 后台路由
    /// </summary>
    public class admin_routing : RazorViewEngine
    {
        public admin_routing()
        {
            //视图位置
            ViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/LindAdmin/{1}/{0}.cshtml"
            };

            //分部视图位置
            PartialViewLocationFormats = new[]
            {
                "~/Views/{1}/{0}.cshtml",
                "~/Views/Shared/{0}.cshtml",
                "~/Views/LindAdmin/Shared/{0}.cshtml",
                "~/Views/LindAdmin/{1}/{0}.cshtml"
            };
        }

        public override ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache)
        {
            return base.FindView(controllerContext, viewName, masterName, useCache);
        }

        public override ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache)
        {
            return base.FindPartialView(controllerContext, partialViewName, useCache);
        }
    }


}