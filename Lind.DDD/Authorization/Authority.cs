using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Authorization
{
    /// <summary>
    /// 权限类型
    /// </summary>
    [Flags]
    public enum Authority
    {
        /// <summary>
        /// 查看，默认值
        /// </summary>
        [Description("查看")]
        Detail = 1,
        /// <summary>
        /// 添加
        /// </summary>
        [Description("添加")]
        Create = 2,
        /// <summary>
        /// 修改
        /// </summary>
        [Description("修改")]
        Edit = 4,
        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 8,
        /// <summary>
        /// 冻结
        /// </summary>
        [Description("冻结")]
        Freeze = 16,
        /// <summary>
        /// 审批
        /// </summary>
        [Description("审批")]
        Approve = 32,
    }
}
