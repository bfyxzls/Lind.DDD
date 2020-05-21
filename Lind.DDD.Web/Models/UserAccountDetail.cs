using Lind.DDD.Web.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Models
{
    /// <summary>
    /// 账户明细，金额来源
    /// </summary>
    public class UserAccountDetail : Lind.DDD.Domain.Entity
    {
        /// <summary>
        /// 账户ID
        /// </summary>
        public int UserAccountId { get; set; }
        /// <summary>
        /// 流向
        /// 0:收入，1:支出
        /// </summary>
        public UserAccountDetailType Type { get; set; }
        /// <summary>
        /// 本次金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string Memo { get; set; }
    }
}
