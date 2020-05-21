using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Domain
{
    /// <summary>
    /// 实体－拥有者行为
    /// 拥有者行为(租户)
    /// </summary>
    public interface IOwnerBehavor
    {
        /// <summary>
        /// 拥有者Id
        /// </summary>
        int OwnerId { get; set; }
        /// <summary>
        /// 拥有者名称
        /// </summary>
        string OwnerName { get; set; }
    }
}
