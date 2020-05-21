using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Enums
{
    /// <summary>
    /// 账户明细
    /// </summary>
    public enum UserAccountDetailType
    {
        [Description("收入")]
        In = 0,
        [Description("支出")]
        Out = 1,
    }
}
