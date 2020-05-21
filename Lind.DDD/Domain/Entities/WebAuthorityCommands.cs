using Lind.DDD.Domain;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Domain
{
    /// <summary>
    /// 命令按钮特性
    /// </summary>
    public enum WebAuthorityCommandFeature
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None,
        /// <summary>
        /// 显示警告
        /// </summary>
        [Description("显示警告")]
        Warn,
        /// <summary>
        /// 以弹框打开
        /// </summary>
        [Description("以弹框打开")]
        Dialog
    }

    /// <summary>
    /// 操作命令实体
    /// </summary>
    public class WebAuthorityCommands : Entity
    {
        /// <summary>
        /// 标识位，64位无符号，操作删除，WebManageRoles_WebManageMenus_Authority_R表的这个位将被回收
        /// </summary>
        [DisplayName("标识位"), Required]
        public long Flag { get; set; }
        /// <summary>
        /// 命令名称（中文）
        /// </summary>
        [DisplayName("按钮名称"), Required(ErrorMessage = "请填写按钮名称")]
        public string Name { get; set; }
        /// <summary>
        /// Action名称，也可以设计成枚举元素名
        /// </summary>
        [DisplayName("Action名称"), Required(ErrorMessage = "请填写Action名称")]
        public string ActionName { get; set; }
        /// <summary>
        /// 标签CSS名称
        /// </summary>
        [DisplayName("标签CSS名称")]
        public string ClassName { get; set; }
        /// <summary>
        /// 特性,0:无,1:操作前显示警告,2:打开页面使用弹框方式
        /// </summary>
        [DisplayName("特性"), Required(ErrorMessage = "请填写特性")]
        public WebAuthorityCommandFeature Feature { get; set; }
        /// <summary>
        /// 是否被使用
        /// </summary>
        [DisplayName("是否被使用"), NotMapped]
        public bool IsUsed { get; set; }
    }
}
