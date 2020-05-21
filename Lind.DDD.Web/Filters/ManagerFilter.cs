using Lind.DDD.Authorization;
using Lind.DDD.Web.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lind.DDD.Web.Filters
{
    /// <summary>
    /// 后台过滤器
    /// </summary>
    public class ManagerFilter : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (CurrentUser.Role != UserRole.Manager.ToString())
            {
                filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary { 
                     { "Action","Setting" },
                     { "Controller", "User"}});
            }
        }
    }
}
