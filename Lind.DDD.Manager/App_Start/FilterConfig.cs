using Lind.DDD.Authorization.Mvc;
using Lind.DDD.Manager.Filters;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Manager
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            //错误日志
            filters.Add(new Lind.DDD.Filters.MvcExceptionErrorLoggerAttribute());
            //用户授权
            filters.Add(new AuthorizationLoginFilter("AdminCommon", "LogOn"));
            //后台菜单权限
            //filters.Add(new ManagerUrlAttribute());
            //Cat拦截
            filters.Add(new Lind.DDD.CatClientPur.CatFilter());
        }
    }
}