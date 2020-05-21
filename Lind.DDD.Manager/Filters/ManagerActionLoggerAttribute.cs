using Lind.DDD.Authorization;
using Lind.DDD.IRepositories;
using Lind.DDD.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Lind.DDD.Utils;
namespace Lind.DDD.Manager.Filters
{
    /// <summary>
    /// Action操作日志特性
    /// </summary>
    public sealed class ManagerActionLoggerAttribute : Lind.DDD.Filters.ActionLoggerAttribute
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="description">功能说明</param>
        /// <param name="param">说明参数，可以为空</param>
        public ManagerActionLoggerAttribute(string description, params string[] param)
            : base((e) =>
            {
                int userid;
                int.TryParse(CurrentUser.UserID, out userid);
                var db = new ManagerContext();
                var repository = new ManagerEfRepository<WebLogger>();
                repository.SetDataContext(db);
                var webLogger = new WebLogger
                {
                    ActionName = e.ActionName,
                    ControllerName = e.ControllerName,
                    Description = e.Descrption,
                    UserId = userid,
                    UserName = CurrentUser.UserName,
                    RequestParams = e.RequestParams,
                    Authority = e.Authority,
                };
                repository.Insert(webLogger);

            }, description, param)
        {
            _description = description;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
        }

        string _description;
        public override string ToString()
        {
            return _description;
        }
    }

}
