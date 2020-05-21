using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Domain
{
    /// <summary>
    /// 实体－状态行为
    /// 实体有状态属性，可以实现这个接口
    /// </summary>
    public interface IStatusBehavor
    {
        /// <summary>
        /// 实体状态
        /// </summary>
        [DisplayName("状态")]
        Status DataStatus { get; set; }
    }
}
