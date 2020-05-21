using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin.Results
{
    /// <summary>
    /// 订单查询结果
    /// </summary>
    public class OrderQueryResult : NotifyResult
    {
        public OrderQueryResult(ResultBuilder resultBuilder)
            : base(resultBuilder)
        { }
        /// <summary>
        /// 交易状态
        /// </summary>
        public TradeState Trade_State { get { return (TradeState)GetEnumValue<TradeState>("trade_state"); } }

    }
}
