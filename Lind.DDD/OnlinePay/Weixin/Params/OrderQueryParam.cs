using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin.Params
{
    /// <summary>
    /// 订单查询接口参数
    /// </summary>
    public class OrderQueryParam
    {
        /// <summary>
        /// 微信订单号  微信的订单号，优先使用  非必填
        /// </summary>
        public string Transaction_Id { get; set; }
        /// <summary>
        /// 商户订单号  商户系统内部的订单号, transaction_id、 out_trade_no二选一，如果同时存在优先级：transaction_id> out_trade_no 
        /// </summary>
        public string Out_Trade_No { get; set; }
    }
}
