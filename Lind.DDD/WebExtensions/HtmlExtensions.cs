using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using System.Linq.Expressions;
using Lind.DDD.Utils;
using System.Web.WebPages;
using System.Web;
using System.Web.Mvc.Html;
namespace Lind.DDD.WebExtensions
{
    /// <summary>
    /// 按钮类型
    /// </summary>
    public enum ButtonType
    {
        Button, Submit, Reset
    }

    /// <summary>
    /// HTML标记扩展
    /// </summary>
    public static class HtmlExtensions
    {
        /// <summary>
        /// 从ModelState中返回指定键对应的验证的错误消息
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString ValidationMessageTextFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression)
        {
            var fieldName = ExpressionHelper.GetExpressionText(expression);
            var errList = new List<string>();
            var modelState = htmlHelper.ViewData.ModelState;
            if (modelState.Keys.Contains(fieldName))
            {
                if (modelState[fieldName].Errors.Count > 0)
                {
                    modelState[fieldName].Errors.ToList().ForEach(i =>
                    {
                        errList.Add(i.ErrorMessage);
                    });
                }
            }
            return MvcHtmlString.Create(string.Join(",", errList));
        }

        #region 超链接的扩展（后台权限设计）
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, Enum operatorAuthority)
        {
            if ((Convert.ToInt32((System.Web.HttpContext.Current.Session["OperatorAuthority"] ?? "0").ToString()) & Convert.ToInt32(operatorAuthority)) == 0)
                return null;
            else
                return htmlHelper.ActionLink(linkText, actionName, null, new { @class = "button" });
        }
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, Enum operatorAuthority)
        {
            if ((Convert.ToInt32((System.Web.HttpContext.Current.Session["OperatorAuthority"] ?? "0").ToString()) & Convert.ToInt32(operatorAuthority)) == 0)
                return null;
            else
                return htmlHelper.ActionLink(linkText, actionName, routeValues, new { @class = "button" });
        }
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, object routeValues, object htmlAttributes, Enum operatorAuthority)
        {
            if ((Convert.ToInt32((System.Web.HttpContext.Current.Session["OperatorAuthority"] ?? "0").ToString()) & Convert.ToInt32(operatorAuthority)) != 0)
                return htmlHelper.ActionLink(linkText, actionName, routeValues, htmlAttributes ?? new { @class = "button" });
            else
                return null;
        }

        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, Enum operatorAuthority)
        {
            if ((Convert.ToInt32((System.Web.HttpContext.Current.Session["OperatorAuthority"] ?? "0").ToString()) & Convert.ToInt32(operatorAuthority)) == 0)
                return null;
            else
                return htmlHelper.ActionLink(linkText, actionName, controllerName, new { @class = "button" });
        }
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, Enum operatorAuthority)
        {
            return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, new { @class = "button" });
        }
        public static MvcHtmlString ActionLink(this HtmlHelper htmlHelper, string linkText, string actionName, string controllerName, object routeValues, object htmlAttributes, Enum operatorAuthority)
        {
            if ((Convert.ToInt32((System.Web.HttpContext.Current.Session["OperatorAuthority"] ?? "0").ToString()) & Convert.ToInt32(operatorAuthority)) == 0)
                return null;
            else
            {
                return htmlHelper.ActionLink(linkText, actionName, controllerName, routeValues, new { @class = "button" });

            }

        }
        #endregion 超链接的扩展（后台权限设计）

        #region Submit按钮扩展
        /// <summary>
        /// 建立一个submit类型的按钮
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="buttonText"></param>
        /// <param name="userOperatorRole"></param>
        /// <returns></returns>
        public static MvcHtmlString CreateButton(this HtmlHelper htmlHelper, string id, string buttonText, ButtonType buttonType, Enum operatorAuthority)
        {
            if ((Convert.ToInt32((System.Web.HttpContext.Current.Session["OperatorAuthority"] ?? "0").ToString()) & Convert.ToInt32(operatorAuthority)) == 0)
                return new MvcHtmlString("<input id='" + id + "' type='" + buttonType.ToString() + "' value='" + buttonText + "' disabled='disabled'/>");
            else
                return new MvcHtmlString("<input id='" + id + "' type='" + buttonType.ToString() + "' value='" + buttonText + "' />");
        }
        /// <summary>
        /// 建立一个submit类型的按钮
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="buttonText"></param>
        /// <param name="operatorAuthority"></param>
        /// <returns></returns>
        public static MvcHtmlString CreateButton(this HtmlHelper htmlHelper, string buttonText, Enum operatorAuthority)
        {
            return CreateButton(htmlHelper, string.Empty, buttonText, ButtonType.Submit, operatorAuthority);
        }
        #endregion

        #region 自定义的超链接，为权限设计

        /// <summary>
        /// 自定义链接，根据包的状态及用户的权限控制按钮是否被显示，产生A标签的ID属性
        /// 自定义链接，带有权限控制的
        /// 用于调用一个JS方法
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText">链接名称</param>
        /// <param name="js">是否是JS方法</param>
        /// <param name="url">JS方法名或URL</param>
        /// <param name="userOperatorRole">权限 </param>
        /// <returns></returns>
        public static MvcHtmlString CreateActionLink(this HtmlHelper htmlHelper, string id, string className, string linkText, bool js, string url, Enum userOperatorRole)
        {

            if ((Convert.ToInt32(System.Web.HttpContext.Current.Session["OperatorAuthority"]) & Convert.ToInt32(userOperatorRole)) != 0)
            {
                if (js)
                {
                    if (string.IsNullOrEmpty(id))
                        return MvcHtmlString.Create("<a class=\"" + className + " button\" href=\"javascript:void(0)\" onclick=\"" + url + ";return false;\")' title=\"" + linkText + "\">" + linkText + "</a>");
                    else
                        return MvcHtmlString.Create("<a id=\"" + id + "\" name=\"" + id + "\" class=\"" + className + " button\" href=\"javascript:void(0)\" onclick=\"" + url + ";return false;\")' title=\"" + linkText + "\">" + linkText + "</a>");
                }
                else
                {
                    return MvcHtmlString.Create("<a  class=\"" + className + " button\" href=\"" + url + "\"  title=\"" + linkText + "\">" + linkText + "</a>");
                }
            }

            else
                return null;
        }
        /// <summary>
        /// 自定义链接，带有权限控制的
        /// 不产生A标签的ID属性
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText"></param>
        /// <param name="js"></param>
        /// <param name="url"></param>
        /// <param name="userOperatorRole"></param>
        /// <returns></returns>
        public static MvcHtmlString CreateActionLink(this HtmlHelper htmlHelper, string linkText, bool js, string url, Enum userOperatorRole)
        {
            return CreateActionLink(htmlHelper, " ", null, linkText, true, url, userOperatorRole);
        }
        /// <summary>
        /// 自定义链接，带有权限控制的
        /// 进行普通的链接跳转
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="linkText">链接名称</param>
        /// <param name="url">链接地址</param>
        /// <param name="userOperatorRole">权限</param>
        /// <returns></returns>
        public static MvcHtmlString CreateActionLink(this HtmlHelper htmlHelper, string linkText, string url, Enum userOperatorRole)
        {
            return CreateActionLink(htmlHelper, " ", null, linkText, false, url, userOperatorRole);
        }

        #endregion 自定义的超链接，为权限设计

        /// <summary>
        /// 得到指定枚举类型元素的Description特性值
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GetDescription(this HtmlHelper htmlHelper, Enum type)
        {
            return type.GetDescription();
        }

        #region 字符串扩展方法
        /// <summary>
        /// 给字符加上样式
        /// </summary>
        /// <param name="reviewContent"></param>
        /// <returns></returns>
        public static string AddCharInColor(this string reviewContent)
        {
            string[] contentArr = reviewContent.Split(new char[3] { '/', '/', '@' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in contentArr)
            {
                if (item.Contains("："))
                {
                    string[] arrItem = item.Split('：');
                    string newChar0 = "<span style='Color:#3FA7CB;Font-Size:12px'>" + arrItem[0] + "</span>";
                    reviewContent = reviewContent.Replace(arrItem[0], newChar0);
                }
            }
            return reviewContent;
        }

        /// <summary>
        /// 去除连续的空格
        /// </summary>
        /// <param name="strWords"></param>
        /// <returns></returns>
        static string GetFields(string strWords)
        {

            Regex replaceSpace = new Regex(@"\s{2,}", RegexOptions.IgnoreCase);
            return replaceSpace.Replace(strWords, " ").Trim();

        }
        public static MvcHtmlString SetHighLighter(this string text, string keyword)
        {
            string startTag = "{starttag}";
            string endTag = "{endtag}";

            if (string.IsNullOrEmpty(keyword)) return MvcHtmlString.Create(text);
            //关键字中英文拆分
            keyword = GetFields(keyword);
            string[] strs = keyword.Split(' ');
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < strs.Length; i++)
            {
                int _Kindex = text.ToLower().IndexOf(strs[i].ToLower());
                if (_Kindex != -1)
                {
                    string replaceKW = text.Substring(_Kindex, strs[i].Length);
                    text = Regex.Replace(text, replaceKW, string.Format("{0}{1}{2}", startTag, replaceKW, endTag), RegexOptions.IgnoreCase);
                }
            }
            text = text.Replace(startTag, "<font color=\"#f00\">").Replace(endTag, "</font>");
            return MvcHtmlString.Create(text);
        }

        /// <summary>
        /// 过滤标签
        /// </summary>
        /// <param name="Htmlstring"></param>
        /// <returns></returns>
        public static string NoHTML(this string Htmlstring)
        {
            //删除脚本
            Htmlstring = Regex.Replace(Htmlstring, @"<script[^>]*?>.*?</script>", "", RegexOptions.IgnoreCase);
            //删除HTML
            Htmlstring = Regex.Replace(Htmlstring, @"<(.[^>]*)>", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"([\r\n])[\s]+", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"-->", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"<!--.*", "", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(quot|#34);", "\"", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(nbsp|#160);", "   ", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            Htmlstring = Regex.Replace(Htmlstring, @"&#(\d+);", "", RegexOptions.IgnoreCase);
            Htmlstring.Replace("<", "");
            Htmlstring.Replace(">", "");
            Htmlstring.Replace("\r\n", "");
            Htmlstring = HttpContext.Current.Server.HtmlEncode(Htmlstring).Trim();
            return Htmlstring;
        }

        /// <summary>
        /// 截取字符
        /// </summary>
        /// <param name="stringHelper"></param>
        /// <param name="maxlength">长度</param>
        /// <param name="OperatorJoin">后缀</param>
        /// <returns></returns>
        public static MvcHtmlString CutString(this string stringHelper, int maxlength, string OperatorJoin)
        {
            if (!string.IsNullOrWhiteSpace(stringHelper))
            {
                if (stringHelper.Length > maxlength)
                {
                    return MvcHtmlString.Create(stringHelper.Substring(0, maxlength) + OperatorJoin);
                }
                return MvcHtmlString.Create(stringHelper);
            }
            return MvcHtmlString.Create(string.Empty);
        }
        /// <summary>
        /// 截取字符
        /// </summary>
        /// <param name="stringHelper"></param>
        /// <param name="maxlength">长度</param>
        /// <returns></returns>
        public static MvcHtmlString CutString(this string stringHelper, int maxlength)
        {
            return CutString(stringHelper, maxlength, string.Empty);
        }
        #endregion

        /// <summary>
        /// 屏蔽HTML代码块里的内容,请内容不能点击
        /// </summary>
        /// <returns></returns>
        public static HelperResult DisabledHtml(this HtmlHelper htmlHelper, Func<string, HelperResult> template)
        {
            return new HelperResult(writer =>
            {
                writer.Write("<div style='margin:0 auto;position:relative;'><div style='position:absolute;left:0px;top:0px;width:100%;height:100%;'></div>"
                    + template.Invoke(null)
                    + "</div>");
            });
        }
    }
}
