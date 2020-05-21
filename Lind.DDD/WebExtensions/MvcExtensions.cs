using Lind.DDD.Paging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Lind.DDD.Utils;
using Lind.DDD.LinqExtensions;
using System.Web.WebPages;
using System.Data.Entity.Core.Objects.DataClasses;
using System.ComponentModel.DataAnnotations.Schema;
using Lind.DDD.Authorization;
using Lind.DDD.TreeHelper;
using System.Web.Mvc.Html;
using Lind.DDD.Domain;
using Lind.DDD.IRepositories;
using System.Web.Routing;
using System.Collections.Specialized;
namespace System.Web.Mvc.Html
{
    /// <summary>
    /// MVC扩展
    /// </summary>
    public static class MvcExtensions
    {
        /// <summary>
        /// 命令仓储
        /// </summary>
        static List<WebAuthorityCommands> AuthorityCommandList;
        public static void ReloadWebAuthorityCommands()
        {
            IRepository<WebAuthorityCommands> repository = Lind.DDD.IoC.ServiceLocator.Instance.GetService<IRepository<WebAuthorityCommands>>();
            if (repository != null)
                AuthorityCommandList = repository.GetModel().ToList();
        }
        #region 分页结果里的DisplayNameFor
        /// <summary>
        /// 显示字段的名称DisplayName的值
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayNameFor<TModel>(
            this HtmlHelper<PagedList<TModel>> html,
            Expression<Func<TModel, object>> expression)
        {
            var p = ExpressionExtensions.Property(expression);
            if (p != null)
            {
                var attr1 = p.GetCustomAttribute(typeof(DisplayNameAttribute));
                var attr2 = p.GetCustomAttribute(typeof(DisplayAttribute));
                if (attr1 != null)
                {
                    return MvcHtmlString.Create(((System.ComponentModel.DisplayNameAttribute)attr1).DisplayName);
                }
                if (attr2 != null)
                {
                    return MvcHtmlString.Create(((DisplayAttribute)attr2).Name);
                }
            }
            return MvcHtmlString.Create(string.Empty);
        }
        #endregion

        #region 枚举的显示与下拉列表
        /// <summary>
        /// 显示枚举字段对应内容的Description
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="html"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString DisplayEnumFor<TModel, TValue>(
            this HtmlHelper<TModel> html,
            Expression<Func<TModel, TValue>> expression)
        {
            string content = string.Empty;
            var metadata = ModelMetadata.FromLambdaExpression(expression, html.ViewData);
            var meta = (Enum)metadata.Model;
            content = meta.GetDescription() ?? metadata.Model.ToString();
            return MvcHtmlString.Create(content);
        }
        /// <summary>
        /// 基于枚举类型的下拉列表
        /// </summary>
        /// <param name="html">html视图上下文</param>
        /// <param name="@enum">枚举类型</param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListEnumFor(
            this HtmlHelper html,
            Type @enum)
        {
            return DropDownListEnumFor(html, null, @enum);
        }
        public static MvcHtmlString DropDownListEnumFor(
           this HtmlHelper html,
           Type @enum,
          bool isDisplayAll)
        {
            return DropDownListEnumFor(html, null, @enum, null, isDisplayAll: isDisplayAll);
        }
        public static MvcHtmlString DropDownListEnumFor(
             this HtmlHelper html,
             string name,
             Type @enum,
            bool isDisplayAll)
        {
            return DropDownListEnumFor(html, name, @enum, null, isDisplayAll);
        }
        public static MvcHtmlString DropDownListEnumFor(
           this HtmlHelper html,
           string name,
           Type @enum)
        {
            return DropDownListEnumFor(html, name, @enum, null, true);
        }
        public static MvcHtmlString DropDownListEnumFor(
             this HtmlHelper html,
             Type @enum,
             string obj,
             bool isDisplayAll)
        {
            return DropDownListEnumFor(html, null, @enum, obj, isDisplayAll);
        }
        public static MvcHtmlString DropDownListEnumFor(
           this HtmlHelper html,
           Type @enum,
           string obj)
        {
            return DropDownListEnumFor(html, null, @enum, obj, true);
        }

        /// <summary>
        /// 基于枚举类型的下拉列表
        /// </summary>
        /// <param name="html"></param>
        /// <param name="name"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static MvcHtmlString DropDownListEnumFor(
            this HtmlHelper html,
            string name,
            Type @enum,
            string obj,
            bool isDisplayAll)
        {
            name = name ?? @enum.Name;
            List<SelectListItem> sl = new List<SelectListItem>();
            foreach (int value in Enum.GetValues(@enum))
            {
                sl.Add(new SelectListItem
                {
                    Text = ((Enum)Enum.Parse(@enum, value.ToString())).GetDescription(),
                    Value = value.ToString(),
                    Selected = (obj == value.ToString())
                });
            }
            if (isDisplayAll)
                sl.Insert(0, new SelectListItem { Text = "全部", Value = string.Empty });

            //构建HTML,MVC的DropDownList对选中状态有问题
            StringBuilder str = new StringBuilder();
            str.Append("<select name=" + name + ">");
            foreach (var item in sl)
            {
                if (item.Selected)
                    str.AppendFormat("<option value={0} selected='true'>{1}</optioin>", item.Value, item.Text);
                else
                    str.AppendFormat("<option value={0}>{1}</optioin>", item.Value, item.Text);
            }
            str.Append("<select>");
            return MvcHtmlString.Create(str.ToString());
        }
        #endregion

        #region 遍历枚举对象，输出为复选框的形式，不包括多个Flags值组成的元素
        /// <summary>
        /// 遍历枚举对象，输出为复选框的形式,不包括多个Flags值组成的元素
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString EnumForCheckboxHtmlTags(this HtmlHelper htmlHelper, Type type, int selected = 1)
        {
            return EnumForCheckboxHtmlTags(htmlHelper, null, type);
        }

        /// <summary>
        ///  遍历枚举对象，输出为复选框的形式,不包括多个Flags值组成的元素
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static MvcHtmlString EnumForCheckboxHtmlTags(this HtmlHelper htmlHelper, string name, Type type, int selected = 1)
        {
            return EnumForCheckboxHtmlTags(htmlHelper, name, type, null);
        }
        /// <summary>
        ///  遍历枚举对象，输出为复选框的形式,不包括多个Flags值组成的元素
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="type"></param>
        /// <param name="className"></param>
        /// <returns></returns>
        public static MvcHtmlString EnumForCheckboxHtmlTags(this HtmlHelper htmlHelper, Type type, string className, int selected = 1)
        {
            return EnumForCheckboxHtmlTags(htmlHelper, null, type, className);
        }
        /// <summary>
        /// 遍历枚举对象，输出为复选框的形式,不包括多个Flags值组成的元素
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static MvcHtmlString EnumForCheckboxHtmlTags(this HtmlHelper htmlHelper, string name, Type type, string className, int selected = 1)
        {
            if (!type.IsEnum)
                throw new ArgumentException("这个方法只支持Enum类型");

            StringBuilder str = new StringBuilder();
            foreach (var item in Enum.GetValues(type))
            {
                if (!string.IsNullOrWhiteSpace((item as Enum).GetDescription()))
                    if (item.GetHashCode() == selected || (selected & item.GetHashCode()) > 0)
                        str.AppendFormat("<span style='padding:5px;'><input class='{3}' type='checkbox' checked='checked' value='{0}' name='{1}' id='{1}{0}' /><label for='{1}{0}' style='font-weight:normal'>{2}</label></span>"
                            , item.GetHashCode()
                            , name ?? type.Name
                            , (item as Enum).GetDescription()
                            , className ?? string.Empty);
                    else
                        str.AppendFormat("<span style='padding:5px;'><input class='{3}' type='checkbox' value='{0}' name='{1}' id='{1}{0}' /><label for='{1}{0}'  style='font-weight:normal'>{2}</label></span>"
                                            , item.GetHashCode()
                                            , name ?? type.Name
                                            , (item as Enum).GetDescription()
                                            , className ?? string.Empty);
            }
            return MvcHtmlString.Create(str.ToString());
        }

        /// <summary>
        /// 将授权命令转为checkbox代码断
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <param name="className"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static MvcHtmlString AuthorityCommandForCheckboxHtmlTags(this HtmlHelper htmlHelper,
            string name,
            string className,
            long selected = 1,
            long menuAuthority = -1)
        {
            ReloadWebAuthorityCommands();
            StringBuilder str = new StringBuilder();
            //当前菜单所具体的命令按钮，默认表示显示所有按钮
            if (menuAuthority == -1)
                menuAuthority = AuthorityCommandList.BinaryOr(i => i.Flag);

            foreach (var item in AuthorityCommandList)
            {
                if ((menuAuthority & item.Flag) == item.Flag)//只显示当前菜单的按钮
                {
                    string checkTag = "";
                    if (item.Flag == selected || (selected & item.Flag) > 0)
                        checkTag = "checked='checked'";

                    str.AppendFormat("<span style='padding:5px;'><input class='{3}' type='checkbox' {4} value='{0}' name='{1}' id='{1}{0}' /><label for='{1}{0}' style='font-weight:normal'>{2}</label></span>"
                        , item.Flag
                        , name
                        , item.Name
                        , className ?? string.Empty
                        , checkTag);
                }

            }
            return MvcHtmlString.Create(str.ToString());
        }

        /// <summary>
        /// 将菜单的命令按钮输出
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="className"></param>
        /// <param name="selected"></param>
        /// <returns></returns>
        public static MvcHtmlString AuthorityCommandForSpanHtmlTags(this HtmlHelper htmlHelper, string name, string className, long selected = 1)
        {
            ReloadWebAuthorityCommands();
            StringBuilder str = new StringBuilder();
            List<string> btnList = new List<string>();
            foreach (var item in AuthorityCommandList)
            {
                if (item.Flag == selected || (selected & item.Flag) > 0)
                    btnList.Add(item.Name);
            }

            str.AppendFormat("<span style='padding:5px;'>{0}</span>", string.Join(",", btnList));
            return MvcHtmlString.Create(str.ToString());
        }

        #endregion

        /// <summary>
        /// 返回当前Url，它支持UrlHasPort参数，线上Ngiux分布式这个值调为0，本地带端口时不需要这个参数
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static string GetCurrentUrl(this HtmlHelper htmlHelper)
        {
            return GetCurrentUrl();
        }
        /// <summary>
        /// 返回当前Url，它支持UrlHasPort参数，线上Ngiux分布式这个值调为0，本地带端口时不需要这个参数
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentUrl()
        {
            string returnUrl = HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Host + HttpContext.Current.Request.Url.PathAndQuery;
            if (System.Configuration.ConfigurationManager.AppSettings["UrlHasPort"] == null || System.Configuration.ConfigurationManager.AppSettings["UrlHasPort"] == "1")
            {
                returnUrl = HttpContext.Current.Request.Url.ToString();
            }
            return returnUrl;
        }
        #region Bootstrap弹层
        /// <summary>
        /// bootstrap风格的弹层
        /// 调用：<a data-toggle='modal' data-target='#LindModal'>测试弹层</a>
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="isBtn"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static MvcHtmlString GenerateDialog(this HtmlHelper htmlHelper, bool isBtn, Func<string, HelperResult> result)
        {
            return GenerateDialog(htmlHelper, "查看", isBtn, result);
        }
        /// <summary>
        /// bootstrap风格的弹层
        /// 调用：<a data-toggle='modal' data-target='#LindModal'>测试弹层</a>
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="title"></param>
        /// <param name="isBtn"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        public static MvcHtmlString GenerateDialog(this HtmlHelper htmlHelper, string title, bool isBtn, Func<string, HelperResult> result)
        {
            string templete = @"<div class='modal fade' id='LindModal' tabindex='-1' role='dialog' aria-labelledby='myModalLabel' aria-hidden='true'>
                               <div class='modal-dialog'>
                                   <div class='modal-content'>
                                       <div class='modal-header'>
                                           <button type='button' class='close'
                                               data-dismiss='modal' aria-hidden='true'>
                                               &times;
                                           </button>
                                           <h4 class='modal-title' id='myModalLabel'>" + title +
                                           @"</h4>
                                       </div>
                                       <div class='modal-body' id='dialogContent'>
                                        " + result.Invoke(null) + "</div>";
            if (isBtn)
            {
                templete +=
                @"<div class='modal-footer'>
                     <button type='button' class='btn btn-warning'
                         data-dismiss='modal'>
                         关闭
                     </button>
                     <button type='button' class='btn btn-primary' id='subBtn'>
                         提交
                     </button>
                  </div>";
            }
            templete +=
            @"</div>
                </div>
                  </div>
                    <script>
                        $('#subBtn').click(function(){$('#dialogContent form').submit();});
                     </script>";
            return MvcHtmlString.Create(templete);

        }
        #endregion

        #region 产生分页标记（前台JS,Bootstrape分页）

        /// <summary>
        /// 生成分页脚本块
        /// 当前页page和每页条目pagesize都在url上进行传递
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="totalRecords"></param>
        /// <returns></returns>
        public static MvcHtmlString GeneratePagger(this HtmlHelper htmlHelper, int totalRecords, NameValueCollection nv = null)
        {
            int page = 0;
            int pageSize = 10;
            //当前页码
            if (htmlHelper.ViewContext.RouteData.Values["page"] != null)
                int.TryParse(htmlHelper.ViewContext.RouteData.Values["page"].ToString(), out page);
            else if (HttpContext.Current.Request.QueryString["page"] != null)
                int.TryParse(HttpContext.Current.Request.QueryString["page"], out page);
            //每面记录数
            if (htmlHelper.ViewContext.RouteData.Values["pageSize"] != null)
                int.TryParse(htmlHelper.ViewContext.RouteData.Values["pageSize"].ToString(), out pageSize);
            else if (HttpContext.Current.Request.QueryString["pageSize"] != null)
                int.TryParse(HttpContext.Current.Request.QueryString["pageSize"], out pageSize);
            if (page <= 0) page = 1;
            RouteValueDictionary route = new RouteValueDictionary();
            if (nv != null)
            {
                if (nv["pageSize"] != null)
                    int.TryParse(nv["pageSize"], out pageSize);//为页数赋值
                foreach (string item in nv.Keys)
                    route.Add(item, nv[item]);

            }

            foreach (string item in htmlHelper.ViewContext.HttpContext.Request.QueryString.Keys)
            {
                route[item] = htmlHelper.ViewContext.HttpContext.Request.QueryString[item];
            }
            string controller = htmlHelper.ViewContext.RouteData.Values["controller"].ToString();
            string action = htmlHelper.ViewContext.RouteData.Values["action"].ToString();
            //当前页,前面和后面显示的页数
            int showNum = 2;
            //最前面和最后面显示的页数
            int lastNum = 0;
            int totalPage = (int)Math.Ceiling((double)((double)totalRecords / (double)pageSize));
            if (page > totalPage) page = totalPage;
            int prevPage = page > 1 ? page - 1 : 1;
            int nextPage = page == totalPage ? totalPage : page + 1;
            long startNum = 0;
            long endNum = 0;

            startNum = pageSize * (page - 1) + 1;
            if (totalPage == page)
            {
                endNum = totalRecords;
            }
            else
            {
                endNum = pageSize * page;
            }
            var url = new UrlHelper(htmlHelper.ViewContext.RequestContext);
            StringBuilder sb = new StringBuilder();
            sb.Append("<nav>");
            sb.Append("<ul class=\"pagination  pagination-sm\">");
            var first = new RouteValueDictionary(route);
            first["page"] = 1;
            var prev = new RouteValueDictionary(route);
            prev["page"] = prevPage;
            var next = new RouteValueDictionary(route);
            next["page"] = nextPage;
            var end = new RouteValueDictionary(route);
            end["page"] = totalPage;
            sb.Append("<li><a href=\"" + url.Action(action, controller, first) + "\">首页</a></li>");

            sb.Append("<li><a href=\"" + url.Action(action, controller, prev) + "\" aria-label=\"Previous\"><span aria-hidden=\"true\">&laquo;</span></a></li>");
            #region 遍历分页页签
            if (page <= showNum + lastNum + 1)
            {

                for (int i = 1; i < page; i++)
                {
                    var temp = new RouteValueDictionary(route);
                    temp["page"] = i;
                    sb.Append("<li><a href=\"" + url.Action(action, controller, temp) + "\">" + i + "</a></li>");
                }
            }
            else
            {
                for (int i = 1; i <= lastNum; i++)
                {
                    var temp = new RouteValueDictionary(route);
                    temp["page"] = i;

                    sb.Append("<li><a href=\"" + url.Action(action, controller, temp) + "\">" + i + "</a></li>");
                }

                sb.Append("<li><a>…</a></li>");

                for (int i = page - showNum; i < page; i++)
                {
                    var temp = new RouteValueDictionary(route);
                    temp["page"] = i;

                    sb.Append("<li><a href=\"" + url.Action(action, controller, temp) + "\">" + i + "</a></li>");
                }

            }


            if (!(page == 1 && totalPage == 1))
                sb.Append("<li class=\"active\"><span>" + page + "<span class=\"sr-only\">(current)</span></span></li>");


            //后半部分
            for (int i = page + 1; i <= page + showNum && i <= totalPage; i++)
            {
                var temp = new RouteValueDictionary(route);
                temp["page"] = i;
                sb.Append("<li><a href=\"" + url.Action(action, controller, temp) + "\">" + i + "</a></li>");
            }

            if (page + showNum + lastNum < totalPage)
            {


                for (int i = totalPage - lastNum + 1; i <= totalPage; i++)
                {
                    var temp = new RouteValueDictionary(route);
                    temp["page"] = i;
                    sb.Append("<li><a href=\"" + url.Action(action, controller, temp) + "\">" + i + "</a></li>");
                }
                sb.Append("<li><a>…</a></li>");
            }
            else
            {

                for (int i = page + showNum + 1; i <= totalPage; i++)
                {
                    var temp = new RouteValueDictionary(route);
                    temp["page"] = i;
                    sb.Append("<li><a href=\"" + url.Action(action, controller, temp) + "\">" + i + "</a></li>");
                }

            }
            #endregion

            sb.Append("<li><a href=\"" + url.Action(action, controller, next) + "\" aria-label=\"Next\"><span aria-hidden=\"true\">&raquo;</span></a></li>");
            sb.Append("<li><a href=\"" + url.Action(action, controller, end) + "\">尾页</a></li>");
            sb.Append("<li><a title='" + totalRecords + "'>共" + totalRecords + "条记录</a></li>");
            sb.Append("</ul>");
            sb.Append("</nav>");
            return MvcHtmlString.Create(sb.ToString());
        }

        /// <summary>
        /// 生成分页脚本块
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="model">数据集合</param>
        /// <returns></returns>
        public static MvcHtmlString GeneratePagger<T>(this HtmlHelper htmlHelper, PagedList<T> model)
        {
            return GeneratePagger(htmlHelper, model.TotalCount, model.AddParameters);
        }
        #endregion

        #region 单选框和复选框的扩展
        /// <summary>
        /// 复选框,selValue为选中项
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IEnumerable<string> selValue)
        {
            return CheckBoxAndRadioFor<object, string>(name, selectList, false, selValue);
        }
        /// <summary>
        /// 复选框,selValue为选中项
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string selValue)
        {
            return CheckBox(htmlHelper, name, selectList, new List<string> { selValue });

        }
        /// <summary>
        /// 复选框
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckBox(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return CheckBox(htmlHelper, name, selectList, new List<string>());
        }
        /// <summary>
        /// 复选框
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckBoxFor(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return CheckBox(htmlHelper, name, selectList, new List<string>());
        }
        /// <summary>
        /// 根据列表输出checkbox
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return CheckBoxFor(htmlHelper, expression, selectList, null);
        }
        /// <summary>
        ///  根据列表输出checkbox,selValue为默认选中的项
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString CheckBoxFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string selValue)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            return CheckBoxAndRadioFor<TModel, TProperty>(name, selectList, false, new List<string> { selValue });
        }
        /// <summary>
        /// 输出单选框和复选框
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="isRadio"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, IEnumerable<string> selValue)
        {
            return CheckBoxAndRadioFor<object, string>(name, selectList, true, selValue);
        }
        /// <summary>
        /// 单选按钮组，seletList为选中项
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList, string selValue)
        {
            return RadioButton(htmlHelper, name, selectList, new List<string> { selValue });
        }
        /// <summary>
        /// 单选按钮组
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="name"></param>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButton(this HtmlHelper htmlHelper, string name, IEnumerable<SelectListItem> selectList)
        {
            return RadioButton(htmlHelper, name, selectList, new List<string>());
        }
        /// <summary>
        ///  根据列表输出radiobutton
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList)
        {
            return RadioButtonFor(htmlHelper, expression, selectList, new List<string>());
        }
        /// <summary>
        ///  根据列表输出radiobutton,selValue为默认选中的项
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, IEnumerable<string> selValue)
        {
            string name = ExpressionHelper.GetExpressionText(expression);
            return CheckBoxAndRadioFor<TModel, TProperty>(name, selectList, true, selValue);
        }
        /// <summary>
        /// 根据列表输出radiobutton,selValue为默认选中的项
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="expression"></param>
        /// <param name="selectList"></param>
        /// <param name="selValue"></param>
        /// <returns></returns>
        public static MvcHtmlString RadioButtonFor<TModel, TProperty>(this HtmlHelper<TModel> htmlHelper, Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> selectList, string selValue)
        {
            return RadioButtonFor(htmlHelper, expression, selectList, new List<string> { selValue });
        }
        static MvcHtmlString CheckBoxAndRadioFor<TModel, TProperty>(
            string name,
            IEnumerable<SelectListItem> selectList,
            bool isRadio,
            IEnumerable<string> selValue)
        {
            StringBuilder str = new StringBuilder();
            int c = 0;
            string check, activeClass;
            string type = isRadio ? "Radio" : "checkbox";

            foreach (var item in selectList)
            {
                c++;
                if (selValue != null && selValue.Contains(item.Value))
                {
                    check = "checked='checked'";
                    activeClass = "style=color:red";
                }
                else
                {
                    check = string.Empty;
                    activeClass = string.Empty;
                }
                str.AppendFormat("<span style='padding-right:5px;'><input type='{3}' value='{0}' name={1} id={1}{2} " + check + "/>", item.Value, name, c, type);
                str.AppendFormat("<label for='{0}{1}' {3}>{2}</lable></span>", name, c, item.Text, activeClass);

            }
            return MvcHtmlString.Create(str.ToString());
        }
        #endregion

        #region 页面代码块级权限设计

        /// <summary>
        /// 生产被授权的代码段，按钮，超链接等
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <param name="roleFlag">角色ID</param>
        /// <param name="template"></param>
        /// <returns></returns>
        public static HelperResult RoleHtmlTags(
            this HtmlHelper htmlHelper,
            long roleFlag,
            Func<string, HelperResult> template)
        {

            var authority = GetCurrentUserAuthority(htmlHelper);

            if ((authority & roleFlag) > 0)
                return new HelperResult(writer =>
                {
                    writer.Write(template.Invoke(null));
                });


            return null;
        }
        /// <summary>
        /// 生成页面的所有按钮，这些按钮需要在建立角色时授权
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        public static MvcHtmlString RoleAuthorityButtonTags(this HtmlHelper htmlHelper, int id = 0, long button = 0)
        {
            ReloadWebAuthorityCommands();
            var htmlStr = new StringBuilder();
            var authority = GetCurrentUserAuthority(htmlHelper);
            AuthorityCommandList.ForEach(cmd =>
            {
                //当button大于0时说明用户自定义控制按钮，这时需要对所有按钮进行过滤，如果button等于0，显示具有权限的按钮
                if ((button > 0 && (button & cmd.Flag) == cmd.Flag && (authority & cmd.Flag) == cmd.Flag)
                    || (button == 0 && (authority & cmd.Flag) == cmd.Flag)
                    )
                {
                    string className = string.IsNullOrWhiteSpace(cmd.ClassName) ? "btn btn-default btn-xs" : cmd.ClassName;
                    switch (cmd.Feature)
                    {
                        case WebAuthorityCommandFeature.None:
                            htmlStr.Append(htmlHelper.ActionLink(cmd.Name, cmd.ActionName, new { id = id }, new { @class = className })).Append("&nbsp;&nbsp;");
                            break;
                        case WebAuthorityCommandFeature.Warn:
                            htmlStr.Append("<a href=\"javascript:void(0)\"  class = \"" + className + "\"  onclick = \"javascript:Modal.confirm({ msg: '你确认要操作吗？' }).on(function (e) { if (e) { location.href = '" + new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(cmd.ActionName, new { id = id }) + "' } })\">" + cmd.Name + "</a>").Append("&nbsp;&nbsp;");
                            break;
                        case WebAuthorityCommandFeature.Dialog:
                            htmlStr.Append("<a data-toggle='modal' data-target='#myModal' href='#' onclick='details(\"" + id + "\",\"" + cmd.ActionName + "\")' class='" + className + "'>" + cmd.Name + "</a>").Append("&nbsp;&nbsp;");
                            break;
                        default:
                            throw new ArgumentException("不是可以接受的值!");
                    }
                }
            });
            return MvcHtmlString.Create(htmlHelper.ToString());
        }
        #endregion

        #region 结果集绑定到Table表格（列表）
        public static MvcHtmlString BindToTable<TModel>(
            this HtmlHelper htmlHelper,
            IEnumerable<TModel> list,
            params Expression<Func<TModel, object>>[] expression)
        {
            return BindToTable(htmlHelper, list, 0, expression);
        }
        /// <summary>
        /// 将分页结果集绑定到页面形成table（列表）
        /// </summary>
        /// <typeparam name="TModel">集合类型</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="list">集合</param>
        /// <param name="button">需要显示的按钮</param>
        /// <param name="expression">要显示的字段</param>
        /// <returns></returns>
        public static MvcHtmlString BindToTable<TModel>(
            this HtmlHelper htmlHelper,
            IEnumerable<TModel> list,
            long button,
            params Expression<Func<TModel, object>>[] expression)
        {
            ReloadWebAuthorityCommands();
            //表的主键
            string primaryKey = ObjectContextExtensions.GetPK<TModel>().Name;
            var htmlStr = new StringBuilder();
            htmlStr.Append("<div class='table-responsive'><Table>");

            //导航属性
            List<Tuple<Type, string>> navigation = new List<Tuple<Type, string>>();
            var t = ExpressionExtensions.FilterPropertyInfo<TModel>(navigation, expression).OrderBy(i => primaryKey);

            //约定排列
            List<string> sortList = new List<string>();
            foreach (var sort in expression)
            {
                string str = sort.Body.ToString();
                if (str.StartsWith("Convert("))
                {
                    str = str.Substring(8, str.Length - 9);
                }
                sortList.Add(str);
            }


            #region 排序后的表头
            SortedList<int, string> tableHead = new SortedList<int, string>();
            foreach (var field in t)
            {
                tableHead.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + field.Name)), GetPropertyDisplayName<TModel>(field));
            }
            foreach (var ext in navigation)
            {
                tableHead.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + ext.Item1.Name + "." + ext.Item2)), GetPropertyDisplayName(ext.Item1, ext.Item1.GetProperty(ext.Item2)));
            }
            #endregion

            #region 表格头
            htmlStr.Append("<tr>");
            htmlStr.Append("<th><input type=\"checkbox\" id=\"checkAll\"></th>");

            foreach (var field in tableHead)
            {
                htmlStr.AppendFormat("<th>{0}</th>", field.Value);
            }

            htmlStr.Append("<th>操作</th");
            htmlStr.Append("</tr>");
            #endregion

            #region 表格体
            foreach (var item in list)
            {
                if (ObjectContextExtensions.GetPK<TModel>() == null)
                    throw new ArgumentException("你的表实体需要显示设置主键，KeyAttribute这个特性");

                var id = typeof(TModel).GetProperty(primaryKey).GetValue(item);
                htmlStr.Append("<tr>");
                htmlStr.AppendFormat("<td><input type=\"checkbox\" name=\"checkId\" value=\"{0}\"></td>", id);

                SortedList<int, object> tableBody = new SortedList<int, object>();

                foreach (var field in t)
                {
                    if (field.PropertyType == typeof(Status))
                    {
                        var val = (Status)field.GetValue(item);
                        var info = "";
                        switch (val)
                        {
                            case Status.Normal:
                                info = val.GetDescription();
                                break;
                            case Status.Hidden:
                                info = "<span style='color:green'>" + val.GetDescription() + "</span>";
                                break;
                            case Status.Freeze:
                                info = "<span style='color:red'>" + val.GetDescription() + "</span>";
                                break;
                            case Status.Deleted:
                                info = "<span style='color:gray'>" + val.GetDescription() + "</span>";
                                break;
                            default:
                                throw new ArgumentException("Status参数有问题！");
                        }
                        tableBody.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + field.Name)), info);
                    }
                    else if (field.PropertyType == typeof(DateTime))
                    {
                        tableBody.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + field.Name)), ((DateTime)field.GetValue(item)).ToString("yyyy-MM-dd HH:mm:ss"));
                    }
                    else if (field.PropertyType == typeof(WebAuthorityCommandFeature))
                    {
                        var val = (WebAuthorityCommandFeature)field.GetValue(item);
                        var info = "";
                        switch (val)
                        {
                            case WebAuthorityCommandFeature.Dialog:
                                info = "<span style='color:green'>" + val.GetDescription() + "</span>";
                                break;
                            case WebAuthorityCommandFeature.Warn:
                                info = "<span style='color:red'>" + val.GetDescription() + "</span>";
                                break;
                            case WebAuthorityCommandFeature.None:
                                info = val.GetDescription();
                                break;
                            default:
                                throw new ArgumentException("WebAuthorityCommandFeature参数有问题！");
                        }
                        tableBody.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + field.Name)), info);
                    }
                    else if (field.PropertyType == typeof(bool))
                    {
                        bool val = (bool)field.GetValue(item);
                        tableBody.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + field.Name)), val ? "是" : "否");
                    }
                    else
                    {
                        tableBody.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + field.Name)), field.GetValue(item));
                    }
                }
                foreach (var ext in navigation)
                {

                    var val = typeof(TModel).GetProperty(ext.Item1.Name) == null ? null : typeof(TModel).GetProperty(ext.Item1.Name).GetValue(item);
                    if (val != null)
                    {
                        var subVal = ext.Item1.GetProperty(ext.Item2).GetValue(val);
                        tableBody.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + ext.Item1.Name + "." + ext.Item2)), subVal);
                    }
                    else
                    {
                        tableBody.Add(sortList.IndexOf(sortList.FirstOrDefault(i => i == "i." + ext.Item1.Name + "." + ext.Item2)), string.Empty);
                    }
                }

                foreach (var body in tableBody)
                {
                    htmlStr.AppendFormat("<td style='word-break: break-all'>{0}</td>", body.Value);
                }
                #region 按钮权限

                var authority = GetCurrentUserAuthority(htmlHelper);

                htmlStr.Append("<td>");
                AuthorityCommandList.ForEach(cmd =>
                {
                    //当button大于0时说明用户自定义控制按钮，这时需要对所有按钮进行过滤，如果button等于0，显示具有权限的按钮
                    if ((button > 0 && (button & cmd.Flag) == cmd.Flag && (authority & cmd.Flag) == cmd.Flag)
                        || (button == 0 && (authority & cmd.Flag) == cmd.Flag)
                        )
                    {
                        string className = string.IsNullOrWhiteSpace(cmd.ClassName) ? "btn btn-default btn-xs" : cmd.ClassName;
                        string dialogName = "btn btn-primary btn-xs";
                        string warnName = "btn btn-danger btn-xs";
                        switch (cmd.Feature)
                        {
                            case WebAuthorityCommandFeature.None:
                                htmlStr.Append(htmlHelper.ActionLink(cmd.Name, cmd.ActionName, new { id = id }, new { @class = className })).Append("&nbsp;&nbsp;");
                                break;
                            case WebAuthorityCommandFeature.Warn:
                                htmlStr.Append("<a href=\"javascript:void(0)\"  class = \"" + warnName + "\"  onclick = \"javascript:Modal.confirm({ msg: '你确认要操作吗？' }).on(function (e) { if (e) { location.href = '" + new UrlHelper(htmlHelper.ViewContext.RequestContext).Action(cmd.ActionName, new { id = id }) + "' } })\">" + cmd.Name + "</a>").Append("&nbsp;&nbsp;");
                                break;
                            case WebAuthorityCommandFeature.Dialog:
                                htmlStr.Append("<a data-toggle='modal' data-target='#myModal' href='#' onclick='details(\"" + id + "\",\"" + cmd.ActionName + "\")' class='" + dialogName + "'>" + cmd.Name + "</a>").Append("&nbsp;&nbsp;");
                                break;
                            default:
                                throw new ArgumentException("不是可以接受的值!");
                        }
                    }
                });

                htmlStr.Append("</td>");

                #endregion

                htmlStr.Append("</tr>");
            }
            #endregion

            htmlStr.Append("</table></div>");

            string js = "<script>function details(id,action){$.ajaxLoading({type:'get',url:'/" + htmlHelper.ViewContext.RouteData.Values["controller"] + "/'+action+'?id='+id,success: function (data) {$('#dialogContent').html(data);} });}$(function(){$('#checkAll').click(function() {$('input[name=\"checkId\"]').prop('checked',this.checked);});var $subBox = $(\"input[name='checkId']\");$subBox.click(function(){$(\"#checkAll\").prop('checked',$subBox.length == $(\"input[name='checkId']:checked\").length ? true : false);});});</script>";
            htmlStr.Append(js);

            return MvcHtmlString.Create(htmlStr.ToString());
        }



        #endregion

        #region private Methods
        /// <summary>
        /// 得到当前用户对于当前URL的操作权限
        /// </summary>
        /// <param name="htmlHelper"></param>
        /// <returns></returns>
        static long GetCurrentUserAuthority(HtmlHelper htmlHelper)
        {
            var currentUrl = "/" + htmlHelper.ViewContext.RouteData.Values["controller"] + "/" + htmlHelper.ViewContext.RouteData.Values["action"];
            var currentAuthority = Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<List<Tuple<int, string, long>>>(CurrentUser.ExtInfo)
                .FirstOrDefault(i => !string.IsNullOrWhiteSpace(i.Item2) && i.Item2.ToLower() == currentUrl.ToLower());
            long authority = 0;
            if (currentAuthority != null)
                authority = currentAuthority.Item3;
            return authority;
        }
        /// <summary>
        /// 泛型版本， 得到属性的DisplayName，如果为空就去接口里找
        /// </summary>
        /// <param name="TModel"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        static string GetPropertyDisplayName(Type TModel, PropertyInfo field)
        {
            string propertyDisplayName = field.Name;
            var tAttr = field.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), false);
            if (tAttr != null && tAttr.Count() > 0)
            {
                propertyDisplayName = (tAttr.FirstOrDefault() as System.ComponentModel.DisplayNameAttribute).DisplayName;
            }
            else
            {
                //类中没有说明，就去接口里找
                foreach (var inter in TModel.GetInterfaces())
                {
                    var ip = inter.GetProperties().FirstOrDefault(i => i.Name == field.Name);
                    if (ip != null)
                    {
                        var ipAttr = ip.GetCustomAttributes(typeof(System.ComponentModel.DisplayNameAttribute), false);
                        if (ipAttr != null && ipAttr.Count() > 0)
                        {
                            propertyDisplayName = (ipAttr.FirstOrDefault() as System.ComponentModel.DisplayNameAttribute).DisplayName;
                        }
                        break;
                    }
                }
            }
            return propertyDisplayName;

        }
        /// <summary>
        /// 得到属性的DisplayName，如果为空就去接口里找
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        static string GetPropertyDisplayName<TModel>(PropertyInfo field)
        {
            return GetPropertyDisplayName(typeof(TModel), field);
        }
        #endregion

        #region 实体绑定到Form表单（添加/编辑）
        /// <summary>
        /// 实体绑定到Form表单（添加/编辑）
        /// </summary>
        /// <typeparam name="TModel">实体类型</typeparam>
        /// <param name="htmlHelper">当前http</param>
        /// <param name="entity">实体</param>
        /// <param name="expression">需要显示的字段</param>
        /// <returns></returns>
        public static MvcHtmlString BindToForm<TModel>(
         this HtmlHelper htmlHelper,
         TModel entity,
         params Expression<Func<TModel, object>>[] expression)
        {
            return BindToFormMore(htmlHelper, entity, null, expression);
        }
        /// <summary>
        /// 实体绑定到Form表单（添加/编辑）
        /// </summary>
        /// <typeparam name="TModel">实体类型</typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="entity">实体</param>
        /// <param name="authority">按钮ID</param>
        /// <param name="expression">包含的字段</param>
        /// <returns></returns>
        public static MvcHtmlString BindToForm<TModel>(
        this HtmlHelper htmlHelper,
        TModel entity,
        long authority,
        params Expression<Func<TModel, object>>[] expression)
        {
            return BindToFormMore(htmlHelper, entity, authority, null, expression);
        }
        /// <summary>
        /// 实体绑定到Form表单（添加/编辑）
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <param name="htmlHelper"></param>
        /// <param name="entity"></param>
        /// <param name="template"></param>
        /// <param name="expression"></param>
        /// <returns></returns>
        public static MvcHtmlString BindToFormMore<TModel>(
        this HtmlHelper htmlHelper,
        TModel entity,
        Func<string, HelperResult> template,
        params Expression<Func<TModel, object>>[] expression)
        {
            return BindToFormMore(htmlHelper, entity, 0, template, expression);
        }
        /// <summary>
        /// 实体绑定到Form表单（添加/编辑）
        /// </summary>
        /// <typeparam name="TModel">实体类型</typeparam>
        /// <param name="htmlHelper">当前http</param>
        /// <param name="entity">实体</param>
        /// <param name="template">扩展信息</param>
        /// <param name="expression">需要显示的字段</param>
        /// <returns></returns>
        public static MvcHtmlString BindToFormMore<TModel>(
            this HtmlHelper htmlHelper,
            TModel entity,
            long button,
            Func<string, HelperResult> template,
            params Expression<Func<TModel, object>>[] expression)
        {
            var htmlStr = new StringBuilder();
            htmlStr.Append("<form method=\"post\" class=\"form-horizontal\">");

            if (!htmlHelper.ViewData.ModelState.IsValid)
            {
                var err = CommonHelper.RemoveHtml(htmlHelper.ValidationSummary(true).ToString());
                htmlStr.Append("<div class=\"form-group\"><div class=\"col-sm-offset-2 col-sm-10\" style=\"color:red\"><div class=\"alert alert-danger\" role=\"alert\" style=\"margin-bottom:0px;\">" + err + "</div></div></div>");
            }

            var t = ExpressionExtensions.FilterPropertyInfo<TModel>(expression);
            foreach (var field in t)
            {
                string propertyName = GetPropertyDisplayName<TModel>(field);
                object value = entity == null ? "" : field.GetValue(entity);
                htmlStr.Append("<div class=\"form-group\">");
                htmlStr.AppendFormat("<label class=\"col-sm-2 control-label\" for=\"{0}\">{1}</label>", field.Name, propertyName);
                htmlStr.AppendFormat("<div class=\"col-sm-5\">");
                if (field.PropertyType == typeof(bool))
                {
                    bool val;
                    bool.TryParse(value.ToString(), out val);
                    if (val)
                    {
                        htmlStr.AppendFormat("<label class=\"radio-inline\"><input type=\"radio\" Name=\"{0}\" value=\"1\" checked>是</label>", field.Name);
                        htmlStr.AppendFormat("<label class=\"radio-inline\"><input type=\"radio\" Name=\"{0}\" value=\"0\">否</label>", field.Name);

                    }
                    else
                    {
                        htmlStr.AppendFormat("<label class=\"radio-inline\"><input type=\"radio\" Name=\"{0}\" value=\"1\">是</label>", field.Name);
                        htmlStr.AppendFormat("<label class=\"radio-inline\"><input type=\"radio\" Name=\"{0}\" value=\"0\" checked>否</label>", field.Name);
                    }

                }
                else
                {
                    htmlStr.AppendFormat("<input type=\"text\" placeholder=\"{0}\" class=\"form-control\" Name=\"{1}\" value=\"{2}\" >", propertyName, field.Name, value);
                }

                htmlStr.AppendFormat("</div>");
                htmlStr.AppendFormat("<div class=\"col-sm-5\">");
                htmlStr.Append("<p class=\"text-danger\">");
                htmlStr.Append(htmlHelper.ValidationMessage(field.Name).ToHtmlString());
                htmlStr.Append("</p>");
                htmlStr.AppendFormat("</div>");
                htmlStr.Append("</div>");
            }

            if (template != null)
            {
                string arr = template.Invoke(null).ToHtmlString();
                htmlStr.Append(arr);
            }
            var authority = GetCurrentUserAuthority(htmlHelper);
            if (button > 0)//进行按钮权限判断
            {
                Func<string, HelperResult> resultFun = (msg) => new HelperResult((s) =>
                {
                    s.Write("<button class='btn btn-default'>提交</button>");
                });
                htmlStr.Append("<div class=\"form-group\"><div class=\"col-sm-offset-2 col-sm-10\">" + MvcExtensions.RoleHtmlTags(htmlHelper, button, resultFun) + "</div></div>");
            }
            else//不采用权限方案
            {
                htmlStr.Append("<div class=\"form-group\"><div class=\"col-sm-offset-2 col-sm-10\"><button class='btn btn-default'>提交</button></div></div>");
            }
            htmlStr.Append("</form>");

            return MvcHtmlString.Create(htmlStr.ToString());
        }
        #endregion

        #region 实体绑定到Div（详细）
        /// <summary>
        /// 实体绑定到Div（详细）
        /// </summary>
        /// <typeparam name="TModel">实体类型</typeparam>
        /// <param name="htmlHelper">当前html</param>
        /// <param name="entity">实体</param>
        /// <param name="expression">需要显示的字段</param>
        /// <returns></returns>
        public static MvcHtmlString BindToDetail<TModel>(
          this HtmlHelper htmlHelper,
          TModel entity,
          params Expression<Func<TModel, object>>[] expression)
        {
            return BindToDetailMore(htmlHelper, entity, null, expression);
        }
        /// <summary>
        /// 实体绑定到Div（详细）
        /// </summary>
        /// <typeparam name="TModel">实体类型</typeparam>
        /// <param name="htmlHelper">当前html</param>
        /// <param name="entity">实体</param>
        /// <param name="template">扩展参数</param>
        /// <param name="expression">需要显示的字段</param>
        /// <returns></returns>
        public static MvcHtmlString BindToDetailMore<TModel>(
            this HtmlHelper htmlHelper,
            TModel entity,
            Func<string, HelperResult> template,
            params Expression<Func<TModel, object>>[] expression)
        {
            var htmlStr = new StringBuilder();
            htmlStr.Append("<form class=\"form-horizontal\">");

            var t = ExpressionExtensions.FilterPropertyInfo<TModel>(expression);
            foreach (var field in t)
            {
                string propertyName = GetPropertyDisplayName<TModel>(field);

                object value = entity == null ? "" : field.GetValue(entity);
                if (field.PropertyType == typeof(Lind.DDD.Domain.Status))
                {
                    value = ((Lind.DDD.Domain.Status)field.GetValue(entity)).GetDescription();
                }

                else if (field.PropertyType == typeof(Lind.DDD.Domain.WebAuthorityCommandFeature))
                {
                    value = ((Lind.DDD.Domain.WebAuthorityCommandFeature)field.GetValue(entity)).GetDescription();
                }
                htmlStr.Append("<div class=\"form-group\">");
                htmlStr.AppendFormat("<label class=\"col-sm-3 control-label\" for=\"{0}\">{1}</label>", field.Name, propertyName);
                htmlStr.AppendFormat("<div class=\"col-sm-9\" style=\"word-break: break-all;\"><p style=\"border-bottom:1px dashed\" class=\"form-control-static\">{0}</p></div>", value);
                htmlStr.Append("</div>");
            }

            if (template != null)
            {
                string arr = template.Invoke(null).ToHtmlString();
                htmlStr.Append(arr);
            }

            htmlStr.Append("</form>");

            return MvcHtmlString.Create(htmlStr.ToString());
        }
        #endregion
    }

}
