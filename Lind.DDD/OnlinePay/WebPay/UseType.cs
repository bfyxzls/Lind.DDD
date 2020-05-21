using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.WebPay
{
    /// <summary>
    /// 使用目的
    /// 1充值，2订单
    /// </summary>
    public enum UseType
    {
        /// <summary>
        /// 充值
        /// </summary>
        Recharge = 1,
        /// <summary>
        /// 订单
        /// </summary>
        Order = 2,
    }
}
