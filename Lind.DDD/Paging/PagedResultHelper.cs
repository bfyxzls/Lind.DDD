using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Collections.Specialized;
using System.Reflection;
using Lind.DDD.Paging;
 

namespace Lind.DDD.Presentation
{

    /// <summary>
    /// 关于PagedResult对象的分页展示
    /// 作者：张占岭，花名：仓储大叔
    /// </summary>
    public static class PagedResultHelper
    {

        #region Ajax分页
        /// <summary>
        /// AJAX分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="pagedList"></param>
        /// <param name="UpdateTargetId"></param>
        /// <returns></returns>
        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedResult<T> pagedList, string UpdateTargetId, bool isDisplayCompletePage)
        {
            var ui = new PageUI(html.ViewContext.RequestContext.HttpContext.Request.Url.ToString(), UpdateTargetId, pagedList.AddParameters);
            if (!isDisplayCompletePage)
                return MvcHtmlString.Create(ui.GetPage(pagedList.PageIndex, pagedList.PageSize, pagedList.TotalRecords, false));
            else
                return MvcHtmlString.Create(ui.GetPage(pagedList.PageIndex, pagedList.PageSize, pagedList.TotalRecords));
        }

        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedResult<T> pagedList, string UpdateTargetId)
        {
            return AjaxPager<T>(html, pagedList, UpdateTargetId, true);
        }
        /// <summary>
        /// AJAX分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="pagedList"></param>
        /// <param name="UpdateTargetId"></param>
        /// <param name="ActionName"></param>
        /// <param name="ControllerName"></param>
        /// <returns></returns>
        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedResult<T> pagedList, string UpdateTargetId, string ActionName, string ControllerName, bool isDisplayCompletePage, bool isTop)
        {
            var mvcUrl = new UrlHelper(html.ViewContext.RequestContext).Action(ActionName, ControllerName); //占岭修改
            var localUrl = string.Format(@"{0}://{1}", html.ViewContext.RequestContext.HttpContext.Request.Url.Scheme, html.ViewContext.RequestContext.HttpContext.Request.Url.Authority);
            var url = string.Format("{0}{1}{2}", localUrl, mvcUrl, html.ViewContext.RequestContext.HttpContext.Request.Url.Query);
            var ui = new PageUI(url, UpdateTargetId, pagedList.AddParameters);
            return MvcHtmlString.Create(ui.GetPage(pagedList.PageIndex, pagedList.PageSize, pagedList.TotalRecords, isDisplayCompletePage, false, isTop));
        }
        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedResult<T> pagedList, string UpdateTargetId, string ActionName, string ControllerName, bool isDisplayCompletePage)
        {
            return AjaxPager<T>(html, pagedList, UpdateTargetId, ActionName, ControllerName, true, false);
        }
        /// <summary>
        /// ajax方式，MVC路由支持的分页
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="pagedList"></param>
        /// <param name="UpdateTargetId"></param>
        /// <param name="ActionName"></param>
        /// <param name="ControllerName"></param>
        /// <returns></returns>
        public static MvcHtmlString AjaxPager<T>(this HtmlHelper html, PagedResult<T> pagedList, string UpdateTargetId, string ActionName, string ControllerName)
        {
            var mvcUrl = new UrlHelper(html.ViewContext.RequestContext).Action(ActionName, ControllerName); //占岭修改
            var localUrl = string.Format(@"{0}://{1}", html.ViewContext.RequestContext.HttpContext.Request.Url.Scheme, html.ViewContext.RequestContext.HttpContext.Request.Url.Authority);
            var url = string.Format("{0}{1}{2}", localUrl, mvcUrl, html.ViewContext.RequestContext.HttpContext.Request.Url.Query);
            var ui = new PageUI(url, UpdateTargetId, pagedList.AddParameters);

            return MvcHtmlString.Create(ui.GetPage(pagedList.PageIndex
               , pagedList.PageSize
               , pagedList.TotalRecords
               , 0
               , new UrlHelper(html.ViewContext.RequestContext)
               , html.ViewContext.RouteData.Values["action"].ToString()
               , html.ViewContext.RouteData.Values["controller"].ToString(), true, false, null));

        }
       
        #endregion

        #region Html分页
        /// <summary>
        /// Html分页,不使用MVC路由
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="pagedList"></param>
        /// <returns></returns>
        public static MvcHtmlString Pager<T>(this HtmlHelper html, PagedResult<T> pagedList)
        {
            return Pager<T>(html, pagedList, false);
        }
        public static MvcHtmlString Pager<T>(this HtmlHelper html, PagedResult<T> pagedList, string className)
        {
            return Pager<T>(html, pagedList, false, className);
        }
        public static MvcHtmlString Pager<T>(this HtmlHelper html, PagedResult<T> pagedList, bool router, string className)
        {
            return Pager<T>(html, pagedList, router, true, className);
        }
        /// <summary>
        /// Html分页，router为true表示走MVC路由
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="html"></param>
        /// <param name="pagedList"></param>
        /// <param name="router">路由</param>
        /// <param name="className">ＣＳＳ类名</param>
        /// <returns></returns>
        public static MvcHtmlString Pager<T>(this HtmlHelper html, PagedResult<T> pagedList, bool router, bool isCompleteDisplay, string className)
        {
            if (pagedList == null)
                return null;
            PageUI ui = new PageUI(html.ViewContext.RequestContext.HttpContext.Request.Url.ToString(), pagedList.AddParameters);
            if (router)
                return MvcHtmlString.Create(ui.GetPage(pagedList.PageIndex
                    , pagedList.PageSize
                    , pagedList.TotalRecords
                    , 0
                    , new UrlHelper(html.ViewContext.RequestContext)
                    , html.ViewContext.RouteData.Values["action"].ToString()
                    , html.ViewContext.RouteData.Values["controller"].ToString(), isCompleteDisplay, false, className));
            return MvcHtmlString.Create(ui.GetPage(pagedList.PageIndex, pagedList.PageSize, pagedList.TotalRecords, isCompleteDisplay, className));
        }
        public static MvcHtmlString Pager<T>(this HtmlHelper html, PagedResult<T> pagedList, bool router)
        {
            return Pager<T>(html, pagedList, router, null);
        }
        #endregion

    }
}
