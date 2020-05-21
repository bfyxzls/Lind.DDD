using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace Lind.DDD.Manager
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            config.Filters.Add(new Lind.DDD.Filters.ExceptionErrorLoggerAttribute());
        }
    }
}
