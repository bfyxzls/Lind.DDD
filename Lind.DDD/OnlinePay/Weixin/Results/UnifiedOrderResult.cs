using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin.Results
{
    /// <summary>
    /// 统一支付接口结果
    /// </summary>
    public class UnifiedOrderResult : ResultBase
    {
        public UnifiedOrderResult(ResultBuilder resultBuilder)
            : base(resultBuilder)
        { }
        /// <summary>
        /// 交易类型
        /// </summary>
        public TradeType? Trade_Type
        {
            get
            {
                return (TradeType?)GetEnumValue<TradeType?>("trade_type");
            }
        }
        /// <summary>
        /// 预支付订单号
        /// </summary>
        public string Prepay_Id { get { return GetValue("prepay_id"); } }
        /// <summary>
        /// 二维码链接
        /// </summary>
        public string Code_Url { get { return GetValue("code_url"); } }
    }
}
