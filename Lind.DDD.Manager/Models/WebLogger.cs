using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.DynamicData;

namespace Lind.DDD.Manager.Models
{
    /// <summary>
    /// 操作日志
    /// </summary>
    [TableAttribute("WebLogger")]
    public partial class WebLogger : Lind.DDD.Domain.EntityBase, Lind.DDD.Domain.IOwnerBehavor
    {
        /// <summary>
        /// 标识列
        /// </summary>
        [DisplayName("编号"), Column("ID"), Required, Key]
        public int Id { get; set; }
        /// <summary>
        /// 操作者ID
        /// </summary>
        [DisplayName("操作者ID")]
        public int UserId { get; set; }
        /// <summary>
        /// 操作者
        /// </summary>
        [DisplayName("操作者")]
        public string UserName { get; set; }
        /// <summary>
        /// 控制器名称
        /// </summary>
        [DisplayName("控制器")]
        public string ControllerName { get; set; }
        /// <summary>
        /// Action名称
        /// </summary>
        [DisplayName("Action")]
        public string ActionName { get; set; }
        /// <summary>
        /// 操作权限
        /// </summary>
        [DisplayName("操作权限")]
        public string Authority { get; set; }
        /// <summary>
        /// 当前请求的Get和Post参数JSON串
        /// </summary>
        [DisplayName("请求参数")]
        public string RequestParams { get; set; }
        /// <summary>
        /// 功能说明
        /// </summary>
        [DisplayName("功能说明")]
        public string Description { get; set; }


        #region IOwnerBehavor 成员

        public int OwnerId
        {
            get;
            set;
        }
        public string OwnerName
        {
            get;
            set;
        }

        #endregion
    }
}
