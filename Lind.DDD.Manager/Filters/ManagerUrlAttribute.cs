using Lind.DDD.Authorization;
using Lind.DDD.Domain;
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

namespace Lind.DDD.Manager.Filters
{
    /// <summary>
    /// 后台URL菜单的权限
    /// 需要考虑到PartialView的问题
    /// </summary>
    public sealed class ManagerUrlAttribute : AuthorizeAttribute
    {

        /// <summary>
        /// 菜单仓储
        /// </summary>
        static IExtensionRepository<WebManageMenus> menuRepository = new ManagerEfRepository<WebManageMenus>();
        /// <summary>
        /// 所有已经定义的菜单项
        /// </summary>
        static List<WebManageMenus> allMenuList = menuRepository.GetModel(i => i.DataStatus == Status.Normal).ToList();

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            bool skipAuthorization = filterContext.ActionDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true) ||
                                     filterContext.ActionDescriptor.ControllerDescriptor.IsDefined(typeof(AllowAnonymousAttribute), inherit: true);
                                   //  filterContext.RequestContext.HttpContext.Request.Url.Host == "localhost";


            //本地跳过这个过滤器
            if (skipAuthorization)
                return;


            //当前用户的菜单ID
            var menuIdArr = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<List<Tuple<int, string, int>>>(CurrentUser.ExtInfo).Select(i => i.Item1);
            //拿到当前用户所收取的菜单
            var menuUrlArr = allMenuList.Where(i => menuIdArr.Contains(i.Id)).Select(i => i.LinkUrl).ToList();
            //当前访问的controllerName
            var controllerName = filterContext.RouteData.Values["controller"].ToString();
            //当前访问的actionName
            var actionName = filterContext.RouteData.Values["action"].ToString();
            //当前的url是否为有效的菜单URL(被存储到数据表里，当里后台菜单，导航菜单等)
            var isValid = allMenuList.FirstOrDefault(i => i.LinkUrl == "/" + controllerName + "/" + actionName) != null;//是否为有效的URL,过滤分布视图
            if (isValid)
            {
                //没有当前URL的权限,跳到登陆页
                if (!menuUrlArr.Contains("/" + controllerName + "/" + actionName))
                {
                    string returnUrl = filterContext.RequestContext.HttpContext.Request.UrlReferrer == null
                        ? "/AdminCommon/LogOn"
                        : filterContext.RequestContext.HttpContext.Request.UrlReferrer.AbsolutePath;
                    filterContext.RequestContext.HttpContext.Response.Write("<div style='text-align:center'><div style='MARGIN-RIGHT: auto;MARGIN-LEFT: auto;width:300px;min-height:150px;border: 5px dashed #000;color: green; font-size: 14px;padding: 5px;text-align: center;vertical-align:middle;'><h2>警告</h2><p>您没有被授权【访问此页面】，请<a href=" + returnUrl + ">单击返回</a></p><p style='color:#000'>时间：" + DateTime.Now + "</p></div></div>");
                    filterContext.RequestContext.HttpContext.Response.End();
                    filterContext.Result = new EmptyResult();//清空当前Action,不执行当前Action代码

                }
            }
        }

    }
}
