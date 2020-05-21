using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Lind.DDD.Paging
{

    public class PageUI
    {
        #region Fields & Properties
        /// <summary>
        /// 页码统一CSS,这是一个全局入口,控制默认状态下面pager样式,如果没有设置,则使用standardPager样式
        /// </summary>
        public static string PagerTheme { get; set; }
        /// <summary>
        /// 是否使用ajax2这种方式加载它
        /// </summary>
        public static bool IsLoadingAjax { get; set; }
        private string _PageUrl;
        private string _UpdateTargetId;
        private NameValueCollection _AddParameters;
        private NameValueCollection _QueryString
        {
            get
            {
                string queryString = new Uri(_PageUrl).Query;
                NameValueCollection coll = new NameValueCollection();
                if (queryString.IndexOf('?') == -1)
                {
                    return coll;
                }
                queryString = queryString.Substring(1);

                foreach (var str in queryString.Split('&'))
                {
                    string[] _strs = str.Split('=');
                    if (_strs.Length == 2)
                    {
                        coll.Add(_strs[0], _strs[1]);
                    }
                }
                return coll;
            }
        }
        private string _AbsolutePath
        {
            get
            {
                //  return new Uri(_PageUrl).GetLeftPart(UriPartial.Path);
                return new Uri(_PageUrl).AbsolutePath;
            }
        }
        #endregion

        #region constructors
        public PageUI(string currentUrl)
            : this(currentUrl, string.Empty)
        {

        }

        public PageUI(string currentUrl, string UpdateTargetId)
        {
            _PageUrl = currentUrl;
            _UpdateTargetId = UpdateTargetId;

        }

        public PageUI(string currentUrl, NameValueCollection addParameters)
            : this(currentUrl, string.Empty, addParameters)
        {
        }

        public PageUI(string currentUrl, string UpdateTargetId, NameValueCollection addParameters)
        {
            _PageUrl = currentUrl;
            _UpdateTargetId = UpdateTargetId;
            _AddParameters = addParameters;
        }

        #endregion

        #region GetPage得到分页的ＨＴＭＬ代码块
        /// <summary>
        /// 获取分页HTML
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">页大小</param>
        /// <param name="count">总数</param>
        /// <returns>分页HTML</returns>
        public string GetPage(int pageIndex, int pageSize, long count)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, true, false);
        }

        public string GetPage(int pageIndex, int pageSize, long count, string className)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, true, false, className);
        }


        public string GetPage(int pageIndex, int pageSize, long count, bool isDisplayCompletePage)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, isDisplayCompletePage, false);
        }

        public string GetPage(int pageIndex, int pageSize, long count, bool isDisplayCompletePage, string className)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, isDisplayCompletePage, false, className);
        }
        public string GetPage(int pageIndex, int pageSize, long count, bool isDisplayCompletePage, string className, bool isTop)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, isDisplayCompletePage, false, className, isTop);
        }

        public string GetPage(int pageIndex, int pageSize, long count, bool isDisplayCompletePage, bool isAppend)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, isDisplayCompletePage, isAppend);
        }
        public string GetPage(int pageIndex, int pageSize, long count, bool isDisplayCompletePage, bool isAppend, bool isTop)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, isDisplayCompletePage, isAppend, isTop);
        }

        public string GetPage(int pageIndex, int pageSize, long count, bool isDisplayCompletePage, bool isAppend, string className)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, isDisplayCompletePage, isAppend, className);
        }
        public string GetPage(int pageIndex, int pageSize, long count, bool isDisplayCompletePage, bool isAppend, string className, bool isTop)
        {
            return GetPage(pageIndex, pageSize, count, 0, null, null, null, isDisplayCompletePage, isAppend, className, isTop);
        }
        /// <summary>
        /// 获取分页HTML(MVC路由的）
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        public string GetPage(int pageIndex, int pageSize, long count, UrlHelper url, string action, string controller)
        {
            return GetPage(pageIndex, pageSize, count, 0, url, action, controller, true, false);
        }
        public string GetPage(int pageIndex, int pageSize, long count, UrlHelper url, string action, string controller, string className)
        {
            return GetPage(pageIndex, pageSize, count, 0, url, action, controller, true, false, className);
        }

        public string GetPage(int pageIndex, int pageSize, long count, UrlHelper url, string action, string controller, bool isDisplayCompletePage)
        {
            return GetPage(pageIndex, pageSize, count, 0, url, action, controller, isDisplayCompletePage, false);
        }
        public string GetPage(int pageIndex, int pageSize, long count, UrlHelper url, string action, string controller, bool isDisplayCompletePage, string classname)
        {
            return GetPage(pageIndex, pageSize, count, 0, url, action, controller, isDisplayCompletePage, false, classname);
        }

        /// <summary>
        /// 获取分页HTML
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="count"></param>
        /// <param name="limitMaxPages"></param>
        /// <returns></returns>
        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, null, null, null, true, false);
        }

        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, bool isDisplayCompletePage)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, null, null, null, isDisplayCompletePage, false);
        }
        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, bool isDisplayCompletePage, bool isTop)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, null, null, null, isDisplayCompletePage, false, isTop);
        }

        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, bool isDisplayCompletePage, string className)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, null, null, null, isDisplayCompletePage, false, className);
        }
        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, bool isDisplayCompletePage, string className, bool isTop)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, null, null, null, isDisplayCompletePage, false, className, isTop);
        }

        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, UrlHelper url, string action, string controller)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, url, action, controller, true, false);
        }
        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, UrlHelper url, string action, string controller, bool isTop)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, url, action, controller, true, false, isTop);
        }
        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, UrlHelper url, string action, string controller, string className)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, url, action, controller, true, false, className);
        }
        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, UrlHelper url, string action, string controller, string className, bool isTop)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, url, action, controller, true, false, className, isTop);
        }

        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, UrlHelper url, string action, string controller, bool isDisplayCompletePage, bool isAppend)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, url, action, controller, isDisplayCompletePage, isAppend, null);
        }
        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, UrlHelper url, string action, string controller, bool isDisplayCompletePage, bool isAppend, bool isTop)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, url, action, controller, isDisplayCompletePage, isAppend, null, isTop);

        }

        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, UrlHelper url, string action, string controller, bool isDisplayCompletePage, bool isAppend, string className)
        {
            return GetPage(pageIndex, pageSize, count, limitMaxPages, url, action, controller, isDisplayCompletePage, isAppend, className, false);
        }
        /// <summary>
        /// 获取分页HTML(MVC路由的）
        /// </summary>
        /// <param name="pageIndex">当前页索引</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="count">总记录数</param>
        /// <param name="limitMaxPages">最大页数</param>
        /// <param name="url">url</param>
        /// <param name="action">mvc-action</param>
        /// <param name="controller">mvc-contrller</param>
        /// <param name="isDisplayCompletePage">是否显示完成分页格式</param>
        /// <param name="isAppend">是否为微博分页格式（记录追加方式）</param>
        /// <param name="className">自定义类名</param>
        /// <returns></returns>
        public string GetPage(int pageIndex, int pageSize, long count, int limitMaxPages, UrlHelper url, string action, string controller, bool isDisplayCompletePage, bool isAppend, string className, bool isTop)
        {
            //当前页,前面和后面显示的页数
            int showNum = 2;
            //最前面和最后面显示的页数
            int lastNum = 0;

            string endHtml = "";

            //总页数
            int totalPage = (int)Math.Ceiling((double)((double)count / (double)pageSize));
            if (totalPage < 1) return "";

            if (isDisplayCompletePage)
            {
                if (limitMaxPages != 0)
                {
                    if (totalPage > limitMaxPages) totalPage = limitMaxPages;
                    endHtml = string.Format("<b>&nbsp;&nbsp;共{0}页</b>", totalPage);
                }
                else
                {
                    endHtml = string.Format("<b>&nbsp;&nbsp;{1}/{2}&nbsp;&nbsp;共{0}条</b>", count, pageIndex, totalPage);
                }
            }

            if (pageIndex <= 0) pageIndex = 1;
            if (pageIndex > totalPage) pageIndex = totalPage;

            StringBuilder sb = new StringBuilder();
            if (isAppend)
            {
                if (pageIndex < totalPage && totalPage > 1)
                {
                    if (isAppend)
                    {
                        if (pageIndex < totalPage && totalPage > 1)
                        {
                            if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                            {

                                sb.Append(GetTagLink(pageIndex + 1, "点击查看更多"));
                            }
                            else
                            {
                                sb.Append(GetTagLink(pageIndex + 1, "点击查看更多", url, action, controller));

                            }
                        }
                    }
                }
            }
            else
            {
                sb.Append(@"
                    <style type='text/css'>
                       .page_Standard {
                           padding: 5px;
                           margin: 0px;
                           text-align: center;
                           font-family: 0px;
                           font-family: Arial;
                           font-size: 12px;
                       }
                       .page_Standard a.cur{
                               background: none repeat scroll 0 0 #036cb4;
                               border: 1px solid #036cb4;
                               color: #fff;
                               font-weight: bold;
                               margin: 2px;
                               padding: 2px 5px;
                           }
                           .page_Standard a {
                               border: #eee 1px solid;
                               padding: 2px 5px;
                               margin: 2px;
                               color: #036cb4;
                               text-decoration: none;
                           }
                       
                               .page_Standard A:hover {
                                   border: #999 1px solid;
                                   color: #666;
                               }
                       
                               .page_Standard A:active {
                                   border: #999 1px solid;
                                   COLOR: #666;
                               }
                       
                           .page_Standard span {
                               border: #036cb4 1px solid;
                               padding: 2px 5px;
                               font-weight: bold;
                               margin: 2px;
                               color: #fff;
                               background: #036cb4;
                           }
                       
                           .page_Standard .disabled {
                               border: #eee 1px solid;
                               padding: 2px 5px;
                               margin: 2px;
                               color: #ddd;
                           }
                    </style>
                ");

                #region 当前pager样式选择
                string currentCss = "page_Standard";
                if (!string.IsNullOrWhiteSpace(className))
                    currentCss = className;
                else if (!string.IsNullOrWhiteSpace(PagerTheme))
                    currentCss = PagerTheme;
                #endregion
                sb.Append("<div style='clear:both'></div><div class=\"" + currentCss + "\">");
                long startNum = 0;
                long endNum = 0;

                startNum = pageSize * (pageIndex - 1) + 1;
                if (totalPage == pageIndex)
                {
                    endNum = count;
                }
                else
                {
                    endNum = pageSize * pageIndex;
                }
                if (isDisplayCompletePage)
                {
                    //前半部分
                    if (pageIndex > 1)
                    {
                        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                        {
                            sb.Append(GetTagLink(1, "首页", isTop));
                            sb.Append(GetTagLink(pageIndex - 1, "上一页", isTop));
                        }
                        else
                        {
                            sb.Append(GetTagLink(1, "首页", url, action, controller, isTop));
                            sb.Append(GetTagLink(pageIndex - 1, "上一页", url, action, controller, isTop));
                        }
                    }
                }

                if (pageIndex <= showNum + lastNum + 1)
                {
                    for (int i = 1; i < pageIndex; i++)
                    {
                        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                        {
                            sb.Append(GetTagLink(i, i.ToString(), isTop));
                        }
                        else
                        {
                            sb.Append(GetTagLink(i, i.ToString(), url, action, controller, isTop));
                        }
                    }
                }
                else
                {
                    for (int i = 1; i <= lastNum; i++)
                    {
                        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                        {
                            sb.Append(GetTagLink(i, i.ToString(), isTop));
                        }
                        else
                        {
                            sb.Append(GetTagLink(i, i.ToString(), url, action, controller, isTop));
                        }
                    }

                    sb.Append("<strong>…</strong>");

                    for (int i = pageIndex - showNum; i < pageIndex; i++)
                    {
                        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                        {
                            sb.Append(GetTagLink(i, i.ToString(), isTop));
                        }
                        else
                        {
                            sb.Append(GetTagLink(i, i.ToString(), url, action, controller, isTop));
                        }
                    }
                }

                //当前页
                if (!(pageIndex == 1 && totalPage == 1)) sb.Append("<a class='num cur'>" + pageIndex.ToString() + "</a>");

                //后半部分
                for (int i = pageIndex + 1; i <= pageIndex + showNum && i <= totalPage; i++)
                {
                    if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                    {
                        sb.Append(GetTagLink(i, i.ToString(), isTop));
                    }
                    else
                    {
                        sb.Append(GetTagLink(i, i.ToString(), url, action, controller, isTop));
                    }
                }

                if (pageIndex + showNum + lastNum < totalPage)
                {
                    sb.Append("<strong>…</strong>");

                    for (int i = totalPage - lastNum + 1; i <= totalPage; i++)
                    {
                        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                        {
                            sb.Append(GetTagLink(i, i.ToString(), isTop));
                        }
                        else
                        {
                            sb.Append(GetTagLink(i, i.ToString(), url, action, controller, isTop));
                        }

                    }
                }
                else
                {
                    for (int i = pageIndex + showNum + 1; i <= totalPage; i++)
                    {
                        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                        {
                            sb.Append(GetTagLink(i, i.ToString(), isTop));
                        }
                        else
                        {
                            sb.Append(GetTagLink(i, i.ToString(), url, action, controller, isTop));
                        }
                    }
                }
                if (isDisplayCompletePage)
                {
                    if (pageIndex < totalPage)
                    {
                        if (string.IsNullOrEmpty(action) || string.IsNullOrEmpty(controller))
                        {
                            sb.Append(GetTagLink(pageIndex + 1, "下一页", isTop));
                            sb.Append(GetTagLink(totalPage, "尾页", isTop));
                        }
                        else
                        {
                            sb.Append(GetTagLink(pageIndex + 1, "下一页", url, action, controller, isTop));
                            sb.Append(GetTagLink(totalPage, "尾页", url, action, controller, isTop));
                        }
                    }
                }
                sb.Append(endHtml);
                sb.Append("</div>");
            }
            return sb.ToString();
        }

        #endregion

        #region GetTagLink分页页码的链接代码块
        private string GetTagLink(int pageIndex, string text, LinkType linkType)
        {
            switch (linkType)
            {
                case LinkType.Previouse:
                    return GetTagLink(pageIndex, text, "pr");
                case LinkType.Number:
                    return GetTagLink(pageIndex, text, "nu");
                case LinkType.Next:
                    return GetTagLink(pageIndex, text, "ne");
                case LinkType.First:
                    return GetTagLink(pageIndex, text, "fi");
                case LinkType.End:
                    return GetTagLink(pageIndex, text, "en");
                case LinkType.Ellipsis:
                    return GetTagLink(pageIndex, text, "el");
                default:
                    return GetTagLink(pageIndex, text, "");
            }
        }

        /// <summary>
        /// 获取分页用的连接(不带样式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="text">显示文字</param>
        /// <returns></returns>
        private string GetTagLink(int pageIndex, string text, bool isTop)
        {
            if (string.IsNullOrEmpty(_UpdateTargetId))
            {
                return "<a class='num pagerCss' href=\"" + GetUrlWithPageArg(pageIndex) + "\">" + text + "</a>";
            }
            if (IsLoadingAjax)
            {
                if (isTop)
                    return "<a class='num pagerCss' href=\"" + GetUrlWithPageArg(pageIndex) + "\" onclick=\"$.ajax2({isTop:true,dataType:'html',url: $(this).attr('href'),success: function (data) {$('#" + _UpdateTargetId + "').html(data);}},'" + _UpdateTargetId + "');event.returnValue=false;return false;\">" + text + "</a>";
                else
                    return "<a class='num pagerCss' href=\"" + GetUrlWithPageArg(pageIndex) + "\" onclick=\"$.ajax2({dataType:'html',url: $(this).attr('href'),success: function (data) {$('#" + _UpdateTargetId + "').html(data);}},'" + _UpdateTargetId + "');event.returnValue=false;return false;\">" + text + "</a>";
            }
            return "<a class='num pagerCss' href=\"" + GetUrlWithPageArg(pageIndex) + "\" onclick=\"$('#" + _UpdateTargetId + "').load($(this).attr('href'));event.returnValue=false;return false;\">" + text + "</a>";
        }
        private string GetTagLink(int pageIndex, string text)
        {
            return GetTagLink(pageIndex, text, false);
        }

        /// <summary>
        /// 添加ＭＶＣ路由的分页
        /// 2012－10－18：修改AddParameters无效问题
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="text"></param>
        /// <returns></returns>
        private string GetTagLink(int pageIndex, string text, UrlHelper url, string action, string controller, bool isTop)
        {
            if (_AddParameters != null)
            {
                foreach (var str in _AddParameters.AllKeys)
                {

                    if (url.RequestContext.RouteData.Values.ContainsKey(str))
                        url.RequestContext.RouteData.Values[str] = _AddParameters[str];
                    else
                        url.RequestContext.RouteData.Values.Add(str, _AddParameters[str]);

                }
                if (url.RequestContext.RouteData.Values.ContainsKey("page"))//更新页码
                    url.RequestContext.RouteData.Values["page"] = pageIndex;


            }
            if (string.IsNullOrEmpty(_UpdateTargetId))
            {
                return "<a  class='num' href=\"" + url.Action(action, controller, url.RequestContext.RouteData.Values) + "\">" + text + "</a>";
            }
            if (IsLoadingAjax)
            {
                if (isTop)
                    return "<a class='num pagerCss' href=\"" + url.Action(action, controller, url.RequestContext.RouteData.Values) + "\" onclick=\"$.ajax2({isTop:true,dataType:'html',url: $(this).attr('href'),success: function (data) {$('#" + _UpdateTargetId + "').html(data);}},'" + _UpdateTargetId + "');event.returnValue=false;return false;\">" + text + "</a>";
                else
                    return "<a class='num pagerCss' href=\"" + url.Action(action, controller, url.RequestContext.RouteData.Values) + "\" onclick=\"$.ajax2({dataType:'html',url: $(this).attr('href'),success: function (data) {$('#" + _UpdateTargetId + "').html(data);}},'" + _UpdateTargetId + "');event.returnValue=false;return false;\">" + text + "</a>";
            }

            return "<a class='num' href=\"" + url.Action(action, controller, url.RequestContext.RouteData.Values) + "\" onclick=\"$('#" + _UpdateTargetId + "').load($(this).attr('href'));event.returnValue=false;return false;\">" + text + "</a>";
        }
        private string GetTagLink(int pageIndex, string text, UrlHelper url, string action, string controller)
        {
            return GetTagLink(pageIndex, text, url, action, controller, false);
        }

        /// <summary>
        /// 获取分页用的连接(带样式)
        /// </summary>
        /// <param name="pageIndex">当前页</param>
        /// <param name="text">显示文字</param>
        /// <param name="className">样式</param>
        /// <returns></returns>
        private string GetTagLink(int pageIndex, string text, string className, bool isTop)
        {
            if (string.IsNullOrEmpty(_UpdateTargetId))
            {
                return "<a class=\"num " + className + "\"" + "href=\"" + GetUrlWithPageArg(pageIndex) + "\">" + text + "</a>";
            }
            if (IsLoadingAjax)
            {
                if (isTop)
                    return "<a class='num pagerCss' href=\"" + GetUrlWithPageArg(pageIndex) + "\" onclick=\"$.ajax2({isTop:true,dataType:'html',url: $(this).attr('href'),success: function (data) {$('#" + _UpdateTargetId + "').html(data);}},'" + _UpdateTargetId + "');event.returnValue=false;return false;\">" + text + "</a>";
                else
                    return "<a class='num pagerCss' href=\"" + GetUrlWithPageArg(pageIndex) + "\" onclick=\"$.ajax2({dataType:'html',url: $(this).attr('href'),success: function (data) {$('#" + _UpdateTargetId + "').html(data);}},'" + _UpdateTargetId + "');event.returnValue=false;return false;\">" + text + "</a>";
            }

            return "<a class=\"num " + className + "\"" + " href=\"" + GetUrlWithPageArg(pageIndex) + "\" onclick=\"$('#" + _UpdateTargetId + "').load($(this).attr('href'));event.returnValue=false;return false;\">" + text + "</a>";
        }
        private string GetTagLink(int pageIndex, string text, string className)
        {
            return GetTagLink(pageIndex, text, className, false);
        }

        /// <summary>
        /// 添加ＭＶＣ路由的分页
        /// 2012－10－18：修改AddParameters无效问题
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="text"></param>
        /// <param name="className"></param>
        /// <param name="action"></param>
        /// <param name="controller"></param>
        /// <returns></returns>
        private string GetTagLink(int pageIndex, string text, string className, UrlHelper url, string action, string controller, bool isTop)
        {

            if (_AddParameters != null)
            {
                foreach (var str in _AddParameters.AllKeys)
                {

                    if (url.RequestContext.RouteData.Values.ContainsKey(str))
                        url.RequestContext.RouteData.Values[str] = _AddParameters[str];
                    else
                        url.RequestContext.RouteData.Values.Add(str, _AddParameters[str]);

                }
                if (url.RequestContext.RouteData.Values.ContainsKey("page"))//更新页码
                    url.RequestContext.RouteData.Values["page"] = pageIndex;
            }


            if (string.IsNullOrEmpty(_UpdateTargetId))
            {

                return "<a class=\"num " + className + "\"" + "href=\"" + url.Action(action, controller, new { page = pageIndex }) + "\">" + text + "</a>";
            }
            if (IsLoadingAjax)
            {
                if (isTop)
                    return "<a class='num pagerCss' href=\"" + GetUrlWithPageArg(pageIndex) + "\" onclick=\"$.ajax2({isTop:true,dataType:'html',url: $(this).attr('href'),success: function (data) {$('#" + _UpdateTargetId + "').html(data);}},'" + _UpdateTargetId + "');event.returnValue=false;return false;\">" + text + "</a>";
                else
                    return "<a class='num pagerCss' href=\"" + GetUrlWithPageArg(pageIndex) + "\" onclick=\"$.ajax2({dataType:'html',url: $(this).attr('href'),success: function (data) {$('#" + _UpdateTargetId + "').html(data);}},'" + _UpdateTargetId + "');event.returnValue=false;return false;\">" + text + "</a>";

            }

            return "<a class=\"num " + className + "\"" + " href=\"" + url.Action(action, controller, new { page = pageIndex }) + "\" onclick=\"$('#" + _UpdateTargetId + "').load($(this).attr('href'));event.returnValue=false;return false;\">" + text + "</a>";
        }

        private string GetTagLink(int pageIndex, string text, string className, UrlHelper url, string action, string controller)
        {
            return GetTagLink(pageIndex, text, className, url, action, controller, false);
        }
        #endregion


        public enum LinkType
        {
            /// <summary>
            /// 第一页
            /// </summary>
            First,
            /// <summary>
            /// 前一页
            /// </summary>
            Previouse,
            /// <summary>
            /// 下一页
            /// </summary>
            Next,
            /// <summary>
            /// 最后一页
            /// </summary>
            End,
            /// <summary>
            /// 数字
            /// </summary>
            Number,
            /// <summary>
            /// 省略号
            /// </summary>
            Ellipsis
        }

        public string GetUrlWithArgument(string argName, string argValue)
        {
            if (string.IsNullOrEmpty(argValue)) return GetUrlWithOutArgument(argName);

            string returnValue = "";
            StringBuilder sb = new StringBuilder();
            foreach (var str in _QueryString.AllKeys)
            {
                if (str != "page" && str != argName)
                {
                    sb.Append("&" + str + "=" + _QueryString[str]);
                }
            }
            returnValue = _AbsolutePath + "?" + argName + "=" + argValue + sb.ToString();
            return returnValue;
        }

        public string GetUrlWithArgument(NameValueCollection coll)
        {
            StringBuilder sb = new StringBuilder();
            string symbol = "?";
            foreach (string str in coll.AllKeys)
            {
                sb.Append(symbol + str + "=" + coll[str]);
                symbol = "&";
            }
            StringBuilder sb2 = new StringBuilder();
            foreach (string str in _QueryString.AllKeys)
            {
                if (str != "page" && !coll.AllKeys.Contains(str))
                {
                    sb2.Append("&" + str + "=" + _QueryString[str]);
                }
            }

            return _AbsolutePath + sb.ToString() + sb2.ToString();
        }

        public string SetUrlArgument(NameValueCollection coll)
        {
            StringBuilder sb = new StringBuilder();
            NameValueCollection myColl = new NameValueCollection(coll);
            foreach (string str in _QueryString.AllKeys)
            {
                if (myColl.AllKeys.Contains(str) == false)
                {
                    myColl.Add(str, _QueryString[str]);
                }
            }
            //myColl.Add(_QueryString);
            string symbol = "?";
            foreach (string str in myColl.AllKeys)
            {
                if (!string.IsNullOrEmpty(myColl[str]))
                {
                    sb.Append(symbol + str + "=" + HttpContext.Current.Server.UrlEncode(myColl[str]));
                    symbol = "&";
                }
            }

            return _AbsolutePath + sb.ToString();
        }

        public string GetUrlWithOutArgument(string argName)
        {
            string returnValue = "";
            StringBuilder sb = new StringBuilder();
            foreach (var str in _QueryString.AllKeys)
            {
                if (str != "page" && str != argName)
                {
                    sb.Append("&" + str + "=" + _QueryString[str]);
                }
            }
            if (sb.Length > 0)
            {
                returnValue = _AbsolutePath + "?" + sb.ToString().Substring(1);
            }
            else
            {
                returnValue = _AbsolutePath;
            }

            return returnValue;
        }

        public string GetUrlWithOutArgument(string[] argName)
        {
            string returnValue = "";
            StringBuilder sb = new StringBuilder();
            foreach (var str in _QueryString.AllKeys)
            {
                if (str != "page" && !argName.Contains(str))
                {
                    sb.Append("&" + str + "=" + _QueryString[str]);
                }
            }
            if (sb.Length > 0)
            {
                returnValue = _AbsolutePath + "?" + sb.ToString().Substring(1);
            }
            else
            {
                returnValue = _AbsolutePath;
            }

            return returnValue;
        }

        private string GetUrlWithPageArg(int pageIndex)
        {
            string returnValue = "";
            NameValueCollection coll = _QueryString;
            if (_AddParameters != null)
            {
                foreach (var str in _AddParameters.AllKeys)
                {
                    if (_QueryString.AllKeys.Contains(str))
                    {
                        coll[str] = HttpContext.Current.Server.UrlEncode(_AddParameters[str]);
                    }
                    else
                    {
                        coll.Remove(str);
                        coll.Add(str, HttpContext.Current.Server.UrlEncode(_AddParameters[str]));
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            foreach (var str in coll.AllKeys)
            {
                if (str != "page")
                {
                    sb.Append("&" + str + "=" + coll[str]);
                }
            }
            if (sb.Length > 0)
            {
                sb.Append("&page=" + pageIndex.ToString());
                returnValue = _AbsolutePath + "?" + sb.ToString().Substring(1);
            }
            else
            {
                sb.Append("?page=" + pageIndex.ToString());
                returnValue = _AbsolutePath + sb.ToString();
            }

            return returnValue;
        }

        private NameValueCollection ConvertToNameValueCollection(string queryString)
        {
            NameValueCollection coll = new NameValueCollection();
            if (queryString.IndexOf('?') == -1)
            {
                return coll;
            }
            queryString = queryString.Substring(1);

            foreach (var str in queryString.Split('&'))
            {
                string[] _strs = str.Split('=');
                coll.Add(_strs[0], _strs[1]);
            }
            return coll;
        }
    }
}
