using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace Lind.DDD.Authorization.Mvc
{
    /// <summary>
    /// 授权过滤器
    /// Function:MVC模式下使用
    /// Author:Lind.zhang
    /// </summary>
    public class AuthorizationLoginFilter : AuthorizeAttribute
    {

        /// <summary>
        /// 验证失败后所指向的控制器和action
        /// 可以在使用特性时为它进行赋值
        /// </summary>
        public AuthorizationLoginFilter(string failControllerName = "Home", string failActionName = "Login")
        {
            _failControllerName = failControllerName;
            _failActionName = failActionName;
        }
        /// <summary>
        /// 出错时要跳转的控制器
        /// </summary>
        string _failControllerName;
        /// <summary>
        /// 出错时要跳转的action
        /// </summary>
        string _failActionName;

        /// <summary>
        /// 拦截action
        /// </summary>
        /// <param name="filterContext"></param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            var returnUrl = MvcExtensions.GetCurrentUrl();

            #region AllowAnonymousAttribute过滤
            //被添加AllowAnonymousAttribute特性的过滤器将不参加AuthorizationLoginFilter的验证
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            var actionType = (((System.Web.Mvc.ReflectedActionDescriptor)(filterContext.ActionDescriptor)).MethodInfo).ReturnType;
            string action = filterContext.RouteData.Values["action"].ToString();
            string controller = filterContext.RouteData.Values["controller"].ToString();
            var parentView = filterContext.Controller.ControllerContext.ParentActionViewContext;
            #endregion

            //为登陆页添加例外，其它页都自动在global.asax里添加到全局过滤器中，MVC3及以后版本支持它
            if (!skipAuthorization)
            {

                //验证失败的策略
                Action failAction = () =>
                {
                    if (parentView == null && actionType == typeof(ActionResult))　//view
                        filterContext.Result = new RedirectToRouteResult("Default", new RouteValueDictionary { 
                            { "Action",_failActionName },
                            { "Controller", _failControllerName}, 
                            { "returnUrl", returnUrl} });
                    else//PartialView
                    {
                        var indexUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority;
                        if (System.Configuration.ConfigurationManager.AppSettings["UrlHasPort"] == "0")
                            indexUrl = HttpContext.Current.Request.Url.Scheme
                                  + "://"
                                  + HttpContext.Current.Request.Url.Host;

                        //防治ajax调用分部视图出现登陆超时，在局部跳转URL的问题
                        filterContext.HttpContext.Response.Write("<script>location.href='"
                            + new UrlHelper(filterContext.RequestContext).Action(_failActionName, _failControllerName, new
                            {
                                returnUrl = indexUrl
                            })
                            + "'</script>");
                        filterContext.HttpContext.Response.End();
                        filterContext.Result = new EmptyResult();//清空当前Action,不执行当前Action代码


                    }
                };

                #region 策略1:app登陆，分发sessionId给客户端，之后每次通讯向服务端发这个id
                var sessionId = filterContext.RequestContext.HttpContext.Request.QueryString["SessionId"];
                if (!string.IsNullOrWhiteSpace(sessionId))
                    if (filterContext.RequestContext.HttpContext.Session.SessionID != sessionId)
                        failAction();
                #endregion

                #region 策略2:web登陆，登陆后保存为CurrentUser
                if (!CurrentUser.IsLogin)
                    failAction();
                #endregion


            }

        }
    }
}