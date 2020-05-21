using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Domain
{
    /// <summary>
    /// 实体－审核行为
    /// </summary>
    public interface IAuditedBehavor
    {
        /// <summary>
        /// 审核人编号
        /// </summary>
        int AuditedUserId { get; set; }
        /// <summary>
        /// 审核人
        /// </summary>
        string AuditedUserName { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        int AuditedStatus { get; set; }
        /// <summary>
        /// 审核流程
        /// </summary>
        string AuditedWorkFlow { get; set; }
    }
}
