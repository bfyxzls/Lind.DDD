using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using Lind.DDD.Domain;
using Lind.DDD.IRepositories;

namespace Lind.DDD.TreeHelper
{
    /// <summary>
    /// 树型结构统一构建与展现
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class DataTree<T> where T : ITree<T>, new()
    {
         
        IEnumerable<T> _list;
        public DataTree(IEnumerable<T> list)
        {
            _list = list.OrderBy(i => i.Level);
        }


        #region 树的格式化返回

        public string CreateDataTree(string name, int selectValue, bool displayAuthority = false, List<Tuple<int, long>> menuAuthority = null)
        {
            return CreateDataTree(name, selectValue: selectValue, controller: "", displayAuthority: displayAuthority, menuAuthority: menuAuthority);
        }
        public string CreateDataTree(string name, int selectValue, string controller, bool displayAuthority = false, List<Tuple<int, long>> menuAuthority = null)
        {
            return CreateDataTree(name, selectValue, isRadio: 0, controller: controller, displayAuthority: displayAuthority, menuAuthority: menuAuthority);
        }
        public string CreateDataTree(string name, int selectValue, int isRadio, string controller, bool displayAuthority = false, List<Tuple<int, long>> menuAuthority = null)
        {
            return CreateDataTree(name, new int[] { selectValue }, 0, isRadio, controller, displayAuthority: displayAuthority, menuAuthority: menuAuthority);
        }
        public string CreateDataTree(string name, int selectValue, int isRadio, string controller, bool onlyLeafButton, bool displayAuthority = false, List<Tuple<int, long>> menuAuthority = null)
        {
            return CreateDataTree(name, new int[] { selectValue }, 0, isRadio, controller, false, onlyLeafButton, displayAuthority: displayAuthority, menuAuthority: menuAuthority);
        }

        public string CreateDataTree(string name, int[] selectValue, int treeID, string controller, bool displayAuthority = false, List<Tuple<int, long>> menuAuthority = null)
        {
            return CreateDataTree(name, selectValue, treeID, 1, controller, displayAuthority: displayAuthority, menuAuthority: menuAuthority);
        }

        public string CreateDataTree(string name, int selectValue, string controller, bool displayButton = true, bool displayAuthority = false, List<Tuple<int, long>> menuAuthority = null)
        {
            return CreateDataTree(name, new int[] { selectValue }, 0, 0, controller, displayButton, displayAuthority: displayAuthority, menuAuthority: menuAuthority);
        }
        public string CreateDataTree(string name, int selectValue, string controller, bool displayButton, bool onlyLeafButton, bool displayAuthority = false, List<Tuple<int, long>> menuAuthority = null)
        {
            return CreateDataTree(name, new int[] { selectValue }, 0, 0, controller, displayButton, onlyLeafButton, displayAuthority: displayAuthority, menuAuthority: menuAuthority);
        }

        public string CreateDataTree(string name, int[] selectValue, int treeID, int radioButton, string controller, bool displayAuthority = false, List<Tuple<int, long>> menuAuthority = null)
        {
            return CreateDataTree(name, selectValue, treeID, radioButton, controller, false, displayAuthority: displayAuthority, menuAuthority: menuAuthority);
        }
        /// <summary>
        /// 构建树对象
        /// </summary>
        /// <param name="name">标记名称</param>
        /// <param name="selectValue">选中的值列表</param>
        /// <param name="level">级别</param>
        /// <param name="radioButton">是否为单选</param>
        /// <param name="controller">控制器</param>
        /// <param name="displayButton">是否需要添加删除编辑按钮</param>
        /// <param name="onlyLeafButton">是否只有叶子结构有控器</param>
        /// <param name="displayAuthority">是否显示权限按钮</param>
        /// <returns></returns>
        public string CreateDataTree(string name,
            int[] selectValue,
            int level,
            int radioButton,
            string controller,
            bool displayButton,
            bool onlyLeafButton = false,
            bool displayAuthority = false,
            List<Tuple<int, long>> menuAuthority = null)
        {
            StringBuilder html = new StringBuilder();

            html.Append("<div id='" + name + "_Tree'><ul style='list-style-type:none'>");
            this.GetTree(html, GetCompleteTree(level), name, selectValue, radioButton, controller, displayButton, onlyLeafButton, displayAuthority, menuAuthority: menuAuthority);
            html.Append("</ul></div>");
            //  html.Append("<script type='text/javascript'> $(function () {$(document).on('click', 'input[name=" + name + "][type=checkbox]', function () {$(this).closest('li').find('ul').find('input[name=" + name + "][type=checkbox]').prop('checked', this.checked);});});</script>");

            html.Append(@"<script type='text/javascript'>
                                $(function () {
                                    $('input[type=checkbox][name=" + name + @"]').each(function () {
                                        if ($(this).prop('checked')) {
                                            $(this).nextAll('span').show();
                                        }
                                    });
                            
                                    $(document).on('click', 'input[type=checkbox][name=" + name + @"]', function () {
                            
                                        $(this).closest('li').find('ul').find('input[name=" + name + @"][type=checkbox]').prop('checked', this.checked);
                                        if ($(this).prop('checked'))
                                            $(this).nextAll('span').show();
                                        else
                                            $(this).nextAll('span').hide();
                            
                                        $(this).closest('li').find('ul').find('input[name=" + name + @"][type=checkbox]').each(function () { 
                                        $(this).each(function () {
                                           if ($(this).prop('checked')) {
                                             $(this).nextAll('span').show(); 
                                             $(this).next('label').css('color', 'red');
                                            }
                                           else {
                                            $(this).nextAll('span').hide();   
                                            $(this).next('label').css('color', '');
                                           } 
                                         }); 
                                       });
                                         
                                        
                                      }); 
                                //菜单的点击，树型选中效果
                                $('input[name=" + name + @"]').click(function () {
                                    var range = false;
                            
                                    //当前选项是否为同级被选中的节点,选中自己，一定要选祖宗，取消自己，不一定取消祖宗
                                    if (this.checked || $(this).closest('ul').find('input[name=" + name + @"]:checked').size() == 0) {
                                        range = true;
                                    }
                                    getParent(this, this.checked, range);
                                })
                                //递归找老祖宗
                                //obj:对象
                                //checked:当前是否选中
                                //range:是否级联到父节点
                                function getParent(obj, checked, range) {
                                    //当前依赖的结点加色
                                    if (checked) {
                                        $(obj).next('label').css('color', 'red');
                                    } else {
                                        $(obj).next('label').css('color', '');
                                    }
                                    var o = $(obj).closest('ul').prevAll(':checkbox');
                            
                                    //所有的祖宗的状态
                                    if (checked || $(obj).closest('ul').find('input[name=" + name + @"]:checked').size() == 0) {
                                        range = true;
                                    } else {
                                        range = false;
                                    }
                            
                                    //是否还有爸爸
                                    if (o.size() > 0 && range) {
                                        $(obj).closest('ul').prevAll(':checkbox').prop('checked', checked);
                                        getParent(o, checked, range);
                                    }
                            
                                }
                            }); </script>");

            return html.ToString();
        }

        private void GetTree(StringBuilder html, T entity, string name, int[] selectValue, string controller)
        {
            GetTree(html, entity, name, selectValue, 0, controller, false);
        }

        /// <summary>
        /// 得到树形结构
        /// </summary>
        /// <param name="html">当前view</param>
        /// <param name="entity">树实体</param>
        /// <param name="name">html元素名称</param>
        /// <param name="selectValue">当前选中值</param>
        /// <param name="radio_checkbox">0表单选，1为多选，-1为无</param>
        /// <param name="controller">控制器</param>
        /// <param name="displayButton">是否显示按钮</param>
        /// <param name="onlyLeafButton">是否为叶子节点才显示单选和复选框</param>
        /// <param name="displayAuthority">是否在叶子结点上显示权限复选框</param>
        private void GetTree(
           StringBuilder html,
           T entity,
           string name,
           int[] selectValue,
           int radio_checkbox,
           string controller,
           bool displayButton,
           bool onlyLeafButton = false,
           bool displayAuthority = false,
           List<Tuple<int, long>> menuAuthority = null
           )
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("name参数不能为空");

            html.Append("<li id='" + name + "_node" + entity.Id + "' style='list-style-type:none'>");

            string check = string.Empty;
            if (selectValue.Contains(entity.Id))
                check = "checked='checked'";
            if (radio_checkbox == 0)
            {
                if (!onlyLeafButton || entity.IsLeaf)
                    html.AppendFormat("<input type='radio' " + check + " id='{0}' name='{1}' Value='{2}' />"
                                       , name + entity.Id, name, entity.Id);
            }
            else if (radio_checkbox == 1)
            {
                if (!onlyLeafButton || entity.IsLeaf)
                    html.AppendFormat("<input type='checkbox' " + check + " id='{0}' name='{1}' Value='{2}' />"
                       , name + entity.Id, name, entity.Id);
            }
            html.AppendFormat("<label style='font-weight:normal' for='{0}'>{1}</label>", name + entity.Id, entity.Name);

            //操作按钮
            if (displayButton)
            {

                html.Append(
              @"<a href='/" + controller + "/Create?id=" + entity.Id + "'>新建</a>" +
              @"<a href='/" + controller + "/Delete?id=" + entity.Id + "' onclick = 'javascript:return confirm(\"确认要删除吗？\") ? true : false;' >删除</a>" +
              @"<a href='/" + controller + "/Edit?id=" + entity.Id + "'>编辑</a>");

            }

            //树对象包含了命令操作
            if (!displayAuthority && entity.Authority > 0)
            {
                html.Append("<span>【");
                html.Append(System.Web.Mvc.Html.MvcExtensions.AuthorityCommandForSpanHtmlTags(null, name + entity.Id + "Authority", null, entity.Authority));
                html.Append("】</span>");
            }

            //添加操作权限,条件为叶子节点，有命令按钮
            if (displayAuthority && entity.IsLeaf && entity.Authority > 0)
            {
                html.Append("<span>【");
                html.Append("<span style='border-bottom:1px dotted #aaa;'>");
                long authorityVal = 1;//默认为查看权限
                if (menuAuthority != null)
                {
                    var authority = menuAuthority.FirstOrDefault(i => i.Item1 == entity.Id);
                    if (authority != null)
                        authorityVal = authority.Item2;
                }
                html.Append(System.Web.Mvc.Html.MvcExtensions.AuthorityCommandForCheckboxHtmlTags(null, name + entity.Id + "Authority", null, authorityVal, entity.Authority));
                html.Append("</span>");
                html.Append("】<span>");
            }

            //子孙递归
            if (entity.Sons != null && entity.Sons.Count() > 0)
            {
                html.Append("<ul style='margin:0px;list-style-type:none'>");
                foreach (var item in entity.Sons)
                {
                    this.GetTree(html, item, name, selectValue, radio_checkbox, controller, displayButton, onlyLeafButton, displayAuthority, menuAuthority: menuAuthority);
                }
                html.Append("</ul>");
            }
            html.Append("</li>");
        }
        #endregion

        #region 树UL-LI封装
        public string CreateTreeUL(T tree)
        {
            StringBuilder str = new StringBuilder();
            GetSubTree(tree);
            str.Append("<ul class=\"nav nav-pills\">");
            GetSubTreeUL(str, tree);
            str.Append("</ul>");
            return str.ToString();
        }

        void GetSubTreeUL(StringBuilder str, T tree)
        {
            var sons = tree.Sons;

            if (sons != null && sons.Count > 0)
            {
                sons.ToList().ForEach(i =>
                {
                    if (i.Level == 1)
                    {
                        if (i.Sons != null && i.Sons.Count > 0)
                        {

                            str.Append("<li class=\"dropdown\"  style=\"float:none\">");
                            str.Append("<a data-submenu=\"\" data-toggle=\"dropdown\" tabindex=\"0\">" + i.Name + "<span class=\"caret\"></span></a>");
                            str.Append("<ul class=\"dropdown-menu\">");
                            GetSubTreeUL(str, i);
                            str.Append("</ul>");
                            str.Append("</li>");
                        }
                        else
                        {
                            str.Append("<li style=\"float:none\">");
                            str.Append("<a href=\"" + i.LinkUrl + "\">" + i.Name + "</a>");
                            str.Append("</li>");
                        }
                    }
                    else
                    {
                        if (i.Sons != null && i.Sons.Count > 0)
                        {

                            str.Append("<li class=\"dropdown-submenu\"  style=\"float:none\">");
                            str.Append("<a tabindex=\"0\">" + i.Name + "</a>");
                            str.Append("<ul class=\"dropdown-menu\">");
                            GetSubTreeUL(str, i);
                            str.Append("</ul>");
                            str.Append("</li>");
                            str.Append("<li class=\"divider\"></li>");
                        }
                        else
                        {
                            str.Append("<li style=\"float:none\">");
                            str.Append("<a href=\"" + i.LinkUrl + "\">" + i.Name + "</a>");
                            //   str.Append(System.Web.Mvc.Html.MvcExtensions.EnumForCheckboxHtmlTags(null, typeof(Lind.DDD.Authorization.Authority)));

                            str.Append("</li>");
                        }
                    }

                });
            }
        }

        #endregion

        #region 树的装载
        private T GetCompleteTree(int level)
        {
            var root = _list.FirstOrDefault(i => i.Level == level);
            if (root == null)
                return new T();
            GetSubTree(root);
            return root;
        }

        private void GetSubTree(T tree)
        {
            List<T> sons = _list.Where(i => i.ParentID == tree.Id).ToList();
            if (sons != null && sons.Count > 0)
            {
                tree.Sons = sons;
                sons.ForEach(i =>
                {
                    i.Father = tree;
                    GetSubTree(i);
                });
            }
        }
        #endregion

        #region 仓储树的删除

        private void GetDelSubTree(T tree, Stack<T> stackList)
        {
            List<T> sons = _list.Where(i => i.ParentID == tree.Id).ToList();
            if (sons != null && sons.Count > 0)
            {

                sons.ForEach(i =>
                {
                    stackList.Push(i);
                });

                sons.ForEach(i =>
                {
                    GetDelSubTree(i, stackList);
                });
            }
        }

        /// <summary>
        /// 仓储树的删除
        /// </summary>
        /// <param name="tree"></param>
        /// <param name="repository"></param>
        public Stack<T> GetDeleteTree(T tree)
        {
            Stack<T> stackList = new Stack<T>();
            stackList.Push(tree);
            GetDelSubTree(tree, stackList);

            return stackList;
        }
        #endregion
    }

}