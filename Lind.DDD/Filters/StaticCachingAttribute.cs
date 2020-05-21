using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace Lind.DDD.Filters
{
    /// <summary>
    /// 静态页缓存
    /// </summary>
    public class StaticCachingAttribute : ActionFilterAttribute
    {
        //TODO:有时间可以去实现一下
        string key;
        int timeOut = 5;
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            base.OnActionExecuting(filterContext);
        }
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            base.OnActionExecuted(filterContext);
        }
    }
}
