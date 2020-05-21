using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    /// <summary>
    /// 用户收藏
    /// </summary>
    public class UserCollection : Lind.DDD.Domain.Entity
    {
        [DisplayName("用户编号"), Required]
        public int UserInfoId { get; set; }
        [DisplayName("商品编号"), Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }
        public UserInfo UserInfo { get; set; }
    }
}
