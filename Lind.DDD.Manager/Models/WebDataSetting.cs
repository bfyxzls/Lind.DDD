using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Manager.Models
{
    /// <summary>
    /// 数据集控制的结果
    /// </summary>
    public class WebDataSetting : Lind.DDD.Domain.EntityBase
    {
        /// <summary>
        /// 标识列
        /// </summary>
        [DisplayName("编号"), Column("ID"), Required, Key]
        public int Id { get; set; }
        [DisplayName("数据控制的类型ID"), Required]
        public int WebDataCtrlId { get; set; }
        [DisplayName("角色编号"), Required]
        public int WebManageRolesId { get; set; }
        [DisplayName("被授予的结果ID集合"), Required]
        public string ObjectIdArr { get; set; }
        [DisplayName("部门编号"), Required]
        public int WebDepartmentsId { get; set; }
        [DisplayName("被授予的名称")]
        public string ObjectNameArr { get; set; }
        public WebDataCtrl WebDataCtrl { get; set; }
        public WebManageRoles WebManageRoles { get; set; }


    }

}
