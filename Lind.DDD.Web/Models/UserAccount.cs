using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    /// <summary>
    /// 个人账户
    /// </summary>
    public class UserAccount : Lind.DDD.Domain.Entity
    {
        /// <summary>
        /// 用户ID
        /// </summary>
        public int UserInfoId { get; set; }
        /// <summary>
        /// 可用金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 冻结金额
        /// </summary>
        public decimal FreezeMoney { get; set; }
        /// <summary>
        /// 账户总金额＝可用金额+冻结金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 明细
        /// </summary>
        public IList<UserAccountDetail> UserAccountDetail { get; set; }
    }
}
