using Lind.DDD.Authorization;
using Lind.DDD.IRepositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Lind.DDD.Filters
{
    /// <summary>
    /// 根据action上的特性和当前controller去与menu.linkUrl+authority进行对比，来判断用户是否有权力访问本action
    /// </summary>
    public sealed class ActionAuthorityAttribute : ActionFilterAttribute
    {

        /// <summary>
        /// 授權值
        /// </summary>
        public long AuthorityValue { get; private set; }

        /// <summary>
        /// 授权初始化，默认为读授权
        /// </summary>
        public ActionAuthorityAttribute()
            : this(1)
        { }

        /// <summary>
        /// 授权初始化
        /// </summary>
        /// <param name="authority">授权枚举</param>
        public ActionAuthorityAttribute(Authority authority)
            : this((long)authority)
        {

        }
        /// <summary>
        /// 授权初始化
        /// </summary>
        /// <param name="authority">授权数值</param>
        public ActionAuthorityAttribute(long authority)
        {
            AuthorityValue |= authority;
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            #region 例外
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true)
                ||
                filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
            //|| 
            //filterContext.RequestContext.HttpContext.Request.Url.Host == "localhost";

            if (skipAuthorization)
                return;
            #endregion

            //当前为正常页面,不是分布视图
            var isValid = false;
            //当前用户的菜单和权限
            var menuAuthority = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<List<Tuple<int, string, int>>>(CurrentUser.ExtInfo);
            //当前控制器对应的权限值
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            var actionName = filterContext.RouteData.Values["action"].ToString();
            //当前权限,先找完全匹配的，如果没有，再找controller匹配的
            var current = menuAuthority.Find(i => !string.IsNullOrWhiteSpace(i.Item2)
                && (("/" + controllerName + "/" + actionName).ToLower()) == (i.Item2.ToLower()));

            //注释下面代码，要求精确匹配/controller/action
            //if (current == null)
            //    current = menuAuthority.Find(i =>
            //        !string.IsNullOrWhiteSpace(i.Item2) &&
            //        i.Item2.Split(new char[] { '/' }).Length > 1 && //至少有一级controller
            //        i.Item2.Split(new char[] { '/' })[1].ToLower() == controllerName.ToLower());

            if (current != null)
            {
                if ((current.Item3 & (long)AuthorityValue) == (long)AuthorityValue)
                {
                    isValid = true;
                }
            }

            if (!isValid)
            {

                string returnUrl = filterContext.RequestContext.HttpContext.Request.UrlReferrer == null ? "/AdminCommon/LogOn" : filterContext.RequestContext.HttpContext.Request.UrlReferrer.AbsolutePath;
                filterContext.RequestContext.HttpContext.Response.Write("<div style='text-align:center'><div style='MARGIN-RIGHT: auto;MARGIN-LEFT: auto;width:300px;min-height:150px;border: 5px dashed #00f;color: red; font-size: 14px;padding: 5px;text-align: center;vertical-align:middle;'><h2>警告</h2><p>您没有被授权此【按钮操作】，请<a href=" + returnUrl + ">单击返回</a></p><p style='color:#000'>时间：" + DateTime.Now + "</p></div></div>");
                filterContext.RequestContext.HttpContext.Response.End();
                filterContext.RequestContext.HttpContext.Response.Clear();
                filterContext.Result = new EmptyResult();//清空当前Action,不执行当前Action代码
            }
            base.OnActionExecuting(filterContext);
        }
    }
}
