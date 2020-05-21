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
    /// 数据集控制的类型
    /// 某个用户对某个表的范围控制
    /// 用户根据当前访问的表，找到控制的查询语句，对数据进行过滤
    /// </summary>
    public class WebDataCtrl : Lind.DDD.Domain.EntityBase
    {
        public WebDataCtrl()
        {
            this.WebDataSetting = new HashSet<WebDataSetting>();
        }
        /// <summary>
        /// 标识列
        /// </summary>
        [DisplayName("编号"), Column("ID"), Required, Key]
        public int Id { get; set; }
        [DisplayName("显示名称"), Required]
        public string DataCtrlName { get; set; }
        [DisplayName("数据控制的类型"), Required]
        public string DataCtrlType { get; set; }
        [DisplayName("数据控制的条件"), Required]
        public string DataCtrlField { get; set; }
        [DisplayName("数据控制条件需要的数据的API"), Required]
        public string DataCtrlApi { get; set; }
        [DisplayName("描述")]
        public string Description { get; set; }
        public ICollection<WebDataSetting> WebDataSetting { get; set; }
    }

}
