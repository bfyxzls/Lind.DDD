using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Domain
{
    /// <summary>
    /// 实体－逻辑删除行为
    /// 具有逻辑删除的接口，实体需要实现这个接口，将IsDeleted实现
    /// 在仓储实现类中，delete方法判断实体是否实现了ILogicDeleteBehavor这个接口，然后再决定是否逻辑删除
    /// </summary>
    public interface ILogicDeleteBehavor
    {
        /// <summary>
        /// 是否已经删除，默认为false
        /// </summary>
        [DisplayName("是否删除")]
        bool IsDeleted { get; set; }
    }
}
