using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Lind.DDD.Utils;
namespace Lind.DDD.Filters
{
    /// <summary>
    /// 操作日志实体
    /// </summary>
    public class ActionLoggerModel
    {
        public string ActionName { get; set; }
        public string ControllerName { get; set; }
        public string Descrption { get; set; }
        public string RequestParams { get; set; }
        public string Authority { get; set; }
    }

    /// <summary>
    /// WebMvc-Action操作日志特性
    /// </summary>
    public class ActionLoggerAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// 功能说明
        /// </summary>
        string _description;
        /// <summary>
        /// 功能说明需要的参数
        /// </summary>
        string[] _param;
        /// <summary>
        /// 空序列化方法
        /// </summary>
        static Action<ActionLoggerModel> NullAction = (msg) => Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info(msg.ToJson());
        /// <summary>
        ///　序列化方法
        /// </summary>
        Action<ActionLoggerModel> _action;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="action">日志序列化逻辑</param>
        /// <param name="description">功能说明</param>
        /// <param name="param">说明参数，可以为空</param>
        public ActionLoggerAttribute(Action<ActionLoggerModel> action, string description, params string[] param)
        {
            _description = description;
            _param = param;
            _action = action;
        }
        public ActionLoggerAttribute(string description, params string[] param)
            : this(NullAction, description, param)
        { }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 例外
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
              filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            if (skipAuthorization)
                return;
            #endregion

            //当前请求上下文
            var request = filterContext.HttpContext.Request;
            var controllerName = filterContext.RequestContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RequestContext.RouteData.Values["action"].ToString();
            //请求参数
            var dicParams = new Dictionary<string, string>();
            //授权
            var authority = filterContext.ActionDescriptor.GetCustomAttributes(false).FirstOrDefault(i => i.GetType() == typeof(ActionAuthorityAttribute)) as ActionAuthorityAttribute;
            //模块说明
            if (_param != null && _param.Length > 0)
            {
                _description = string.Format(_description, _param);
            }
            foreach (var item in request.QueryString.AllKeys)
            {
                if (!dicParams.ContainsKey(item))
                    dicParams.Add(item, request.QueryString[item]);
            }
            foreach (var item in request.Form.AllKeys)
            {
                if (!dicParams.ContainsKey(item))
                    dicParams.Add(item, request.Form[item]);
            }

            //持久化到操作日志表
            if (!string.IsNullOrWhiteSpace(_description))
                _action(new ActionLoggerModel
                {
                    ActionName = actionName,
                    ControllerName = controllerName,
                    Descrption = _description,
                    RequestParams = dicParams.ToJson(),
                    Authority = authority == null ? string.Empty : authority.AuthorityValue.ToString()
                });

        }

    }
}
