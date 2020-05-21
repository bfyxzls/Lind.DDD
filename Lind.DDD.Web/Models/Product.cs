using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    public class Product : Lind.DDD.Domain.Entity
    {
        [DisplayName("资源"), Required]
        public string Name { get; set; }
        [DisplayName("价格"), Required]
        public decimal Price { get; set; }
        [DisplayName("折扣"), Required]
        public int Discount { get; set; }
        [DisplayName("描述")]
        public string Description { get; set; }
        [DisplayName("分类"), Required]
        public int CategoryId { get; set; }
        [DisplayName("所有者ID"), Required]
        public int UserInfoId { get; set; }
        [DisplayName("所有者"), Required]
        public string UserInfoUserName { get; set; }

        public Category Category { get; set; }
        public IList<UserCollection> UserCollection { get; set; }
    }
}
