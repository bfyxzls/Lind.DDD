using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    /// <summary>
    /// 购物车,存储到客户端cookies
    /// </summary>
    [NotMapped]
    public class Cart
    {
        [DisplayName("商品ID")]
        public int ProductId { get; set; }
        [DisplayName("商品名称")]
        public string ProductName { get; set; }
        [DisplayName("商品价格")]
        public decimal Price { get; set; }
    }
}
