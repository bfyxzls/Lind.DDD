using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace Lind.DDD.TreeHelper
{
    /// <summary>
    /// 树结果接口
    /// </summary>
    public interface ITree
    {
        /// <summary>
        /// 主键
        /// </summary>
        int Id { get; }
        /// <summary>
        /// 父ID
        /// </summary>
        int? ParentID { get; }
        /// <summary>
        /// 名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 级别，树的根为0
        /// </summary>
        int Level { get; }
        /// <summary>
        /// 是否为叶子节点
        /// </summary>
        bool IsLeaf { get; }
        /// <summary>
        /// 链接地址
        /// </summary>
        string LinkUrl { get; set; }
        /// <summary>
        /// 树上显示的按钮，位运算
        /// </summary>
        long Authority { get; set; }
    }

    /// <summary>
    /// 树结果接口,对泛型的支持
    /// </summary>
    public interface ITree<T> : ITree
    {
        /// <summary>
        /// 父级对象
        /// </summary>
        T Father { get; set; }
        /// <summary>
        /// 子孙对象
        /// </summary>
        IList<T> Sons { get; set; }
    }
}