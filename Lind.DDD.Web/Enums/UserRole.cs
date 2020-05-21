using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Enums
{
    /// <summary>
    /// 角色
    /// </summary>
    public enum UserRole
    {
        [Description("管理员")]
        Manager,
        [Description("普通用户")]
        User,
    }
}
