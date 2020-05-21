using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin.Results
{

    /// <summary>
    /// 通知结果
    /// 自定义参数可以加到Attach里
    /// </summary>
    public class NotifyResult : ResultBase
    {
        public NotifyResult(ResultBuilder resultBuilder)
            : base(resultBuilder)
        { }
        /// <summary>
        /// 设备号
        /// </summary>
        public string Device_Info { get { return GetValue("device_info"); } }
        /// <summary>
        /// 用户标识
        /// </summary>
        public string OpenId { get { return GetValue("openid"); } }
        /// <summary>
        /// 是否关注公众号
        /// </summary>
        public bool Is_Subscribe { get { return GetBooleanValue("is_subscribe"); } }
        /// <summary>
        /// 交易类型
        /// </summary>
        public TradeType Trade_Type { get { return (TradeType)GetEnumValue<TradeType>("trade_type"); } }
        /// <summary>
        /// 付款银行
        /// </summary>
        public string Bank_Type { get { return GetValue("bank_type"); } }
        /// <summary>
        /// 总金额
        /// </summary>
        public int Total_Fee { get { return GetIntValue("total_fee"); } }
        /// <summary>
        /// 现金券金额
        /// </summary>
        public int Coupon_Fee { get { return GetIntValue("coupon_fee"); } }
        /// <summary>
        /// 货币种类  货币类型，符合 ISO 4217标准的三位字母代码，默认人民币：CNY
        /// </summary>
        public string Fee_Type { get { return GetValue("fee_type"); } }
        /// <summary>
        /// 微信支付订单号
        /// </summary>
        public string Transaction_Id { get { return GetValue("transaction_id"); } }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string Out_Trade_No { get { return GetValue("out_trade_no"); } }
        /// <summary>
        /// 商家数据包
        /// </summary>
        public string Attach { get { return GetValue("attach"); } }
        /// <summary>
        /// 支付完成时间
        /// </summary>
        public string Time_End { get { return GetValue("time_end"); } }
        /// <summary>
        /// 产品ID
        /// </summary>
        public string Product_Id { get { return GetValue("product_id"); } }

    }

}
