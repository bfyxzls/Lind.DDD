using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    public class OrderDetail : Lind.DDD.Domain.Entity
    {
        [DisplayName("价格"), Required]
        public decimal Price { get; set; }
        [DisplayName("购买数量"), Required]
        public int SaleCount { get; set; }
        [DisplayName("商品ID"), Required]
        public int ProductId { get; set; }
        [DisplayName("商品名称"), Required]
        public string ProductName { get; set; }
        [DisplayName("商品所有者ID"), Required]
        public int UserInfoId { get; set; }
        [DisplayName("商品所有者"), Required]
        public string UserInfoUserName { get; set; }
        [DisplayName("服务开始时间"), Required]
        public DateTime StartTime { get; set; }
        [DisplayName("服务结束时间"), Required]
        public DateTime EndTime { get; set; }

        public Product Product { get; set; }
    }
}
