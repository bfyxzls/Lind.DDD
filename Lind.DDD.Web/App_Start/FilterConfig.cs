using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //全局错误过滤器
            filters.Add(new HandleErrorAttribute());
            filters.Add(new Lind.DDD.Authorization.Mvc.AuthorizationLoginFilter());

        }
    }
}