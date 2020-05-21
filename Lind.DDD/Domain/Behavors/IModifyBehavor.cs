using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Domain
{
    /// <summary>
    /// 实体－修改行为
    /// 修改数据的行为
    /// 实体实现了接口后，可以在数据提交时自动更新这两个属性
    /// </summary>
    public interface IModifyBehavor
    {
        /// <summary>
        /// 最后更新时间
        /// </summary>
        [DisplayName("最后更新时间")]
        DateTime LastModifyTime { get; set; }
        /// <summary>
        /// 最后操作人ID
        /// </summary>
        [DisplayName("最后修改操作人")]
        int LastModifyUserId { get; set; }
        /// <summary>
        /// 最后操作人名称
        /// </summary>
        string LastModifyUserName { get; set; }
    }
}
