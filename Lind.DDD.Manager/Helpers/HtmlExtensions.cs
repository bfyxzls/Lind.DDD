using Lind.DDD.Authorization;
using Lind.DDD.LinqExtensions;
using Lind.DDD.Manager;
using Lind.DDD.Manager.Models;
using Lind.DDD.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.WebPages;

namespace System.Web.Mvc.Html
{
    /// <summary>
    /// Lind.DDD.Manager扩展方法
    /// </summary>
    public static class HtmlExtensions
    {
        #region 仓储
        /// <summary>
        /// 菜单仓储
        /// </summary>
        static Lind.DDD.IRepositories.IExtensionRepository<Lind.DDD.Manager.Models.WebManageMenus> menuRepository;
       

        #endregion


        static HtmlExtensions()
        {
            menuRepository = new ManagerEfRepository<WebManageMenus>();

        }

        /// <summary>
        /// 构建菜单树
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString GeneratorMenuTree(this HtmlHelper html, int[] selValue, bool displayAuthority = false, List<Tuple<int, long>> authority = null)
        {
            return GeneratorMenuTree(html, selValue, false, displayAuthority, authority);
        }
        public static MvcHtmlString GeneratorMenuTree(this HtmlHelper html)
        {
            return GeneratorMenuTree(html, new int[] { 0 }, false, null);
        }

        public static MvcHtmlString GeneratorMenuTree(this HtmlHelper html, Expression<Func<WebManageMenus, bool>> predicate)
        {
            return GeneratorMenuTree(html, predicate, 1);
        }

        public static MvcHtmlString GeneratorMenuTree(this HtmlHelper html, Expression<Func<WebManageMenus, bool>> predicate, int radio_checkbox)
        {
            return MvcHtmlString.Create(new MenuTree(predicate).CreateDataTree(
              name: "menu",
              selectValue: new int[] { 0 },
              level: 0,
              radioButton: radio_checkbox,
              controller: (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString(),
              displayButton: false,
              onlyLeafButton: false,
              displayAuthority: false,
              menuAuthority: null));
        }
        public static MvcHtmlString GeneratorMenuTree(this HtmlHelper html, bool IsCurd, bool displayAuthority = false, List<Tuple<int, long>> authority = null)
        {
            return GeneratorMenuTree(html, new int[] { 0 }, IsCurd, displayAuthority, authority);
        }
        public static MvcHtmlString GeneratorMenuTree(this HtmlHelper html, int[] selValue, bool IsCurd, bool displayAuthority = false, List<Tuple<int, long>> authority = null)
        {
            return MvcHtmlString.Create(new MenuTree().CreateDataTree(
                name: "menu",
                selectValue: selValue,
                level: 0,
                radioButton: 1,
                controller: (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString(),
                displayButton: IsCurd,
                onlyLeafButton: false,
                displayAuthority: displayAuthority,
                menuAuthority: authority));
        }



        #region 区域的树
        public static MvcHtmlString GeneratorAreaTree(this HtmlHelper html)
        {
            return MvcHtmlString.Create(new AreaTree().CreateDataTree("area", -1, isRadio: 1, controller: (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString()));
        }
        public static MvcHtmlString GeneratorAreaTree(this HtmlHelper html, Expression<Func<WebCommonAreas, bool>> predicate)
        {
            return MvcHtmlString.Create(new AreaTree(predicate).CreateDataTree("area", -1, isRadio: 1, controller: (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString()));
        }
        public static MvcHtmlString GeneratorAreaTree(this HtmlHelper html, Expression<Func<WebCommonAreas, bool>> predicate, int radio_checkbox)
        {
            return MvcHtmlString.Create(new AreaTree(predicate).CreateDataTree("area", -1, isRadio: radio_checkbox, controller: (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString()));
        }
        public static MvcHtmlString GeneratorAreaTree(this HtmlHelper html, int[] selValue)
        {
            return MvcHtmlString.Create(new AreaTree().CreateDataTree("area", selValue, 0, 1, (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString()));
        }
        #endregion

        #region 部门的树
        /// <summary>
        /// 构建部门树checkbox，带curd操作
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString GeneratorDeptTree(this HtmlHelper html, int selValue = 0)
        {
            return MvcHtmlString.Create(new DeptTree().CreateDataTree("dept", selValue, (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString(), true, false, null));
        }
        public static MvcHtmlString GeneratorDeptTreeRadioLeaf(this HtmlHelper html, int selValue = 0)
        {
            return MvcHtmlString.Create(new DeptTree().CreateDataTree("dept", selValue, isRadio: 0, controller: (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString(), displayAuthority: true, menuAuthority: null));
        }
        /// <summary>
        /// 构建部门树radio，不带curd操作
        /// </summary>
        /// <param name="html"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString GeneratorDeptTreeRadio(this HtmlHelper html)
        {
            return GeneratorDeptTreeRadio(html, 0);
        }
        /// <summary>
        ///  构建部门树,不带curd操作
        /// </summary>
        /// <param name="html"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString GeneratorDeptTreeRadio(this HtmlHelper html, int selValue = 0, int isRadio = 0)
        {
            return MvcHtmlString.Create(new DeptTree().CreateDataTree("dept", selValue, isRadio: isRadio, controller: (html.ViewContext.RouteData.Values["controller"] ?? "/").ToString(), displayAuthority: false, menuAuthority: null));
        }
        #endregion

        /// <summary>
        /// 找菜单的爸爸
        /// </summary>
        /// <param name="menu"></param>
        private static void FindFatherTree(Lind.DDD.Manager.Models.WebManageMenus menu, StringBuilder str)
        {
            menu.Father = menuRepository.Find(i => i.Id == menu.ParentID);
            if (menu.Father != null)
            {
                str.Insert(0, string.Format("<li>{0}</li>", menu.Father.Name));
                FindFatherTree(menu.Father, str);
            }
        }

        /// <summary>
        /// 构建面包屑
        /// </summary>
        /// <param name="html"></param>
        /// <returns></returns>
        public static MvcHtmlString BreadCrumb(this System.Web.Mvc.HtmlHelper html)
        {
            StringBuilder str = new StringBuilder();
            string controllerName = (html.ViewContext.RouteData.Values["controller"].ToString() ?? "/");
            string actionName = (html.ViewContext.RouteData.Values["action"].ToString() ?? "/");
            string url = "/" + controllerName + "/" + actionName;
            var entity = menuRepository.Find(i => url.ToLower() == i.LinkUrl.ToLower());
            if (entity != null)
            {
                str.AppendFormat("<li class=\"active\">{0}</li>", entity.Name);
                FindFatherTree(entity, str);
            }
            return MvcHtmlString.Create(str.ToString());
        }




    }
}
