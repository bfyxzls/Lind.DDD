using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.WebPay
{
    /// <summary>
    /// 订单事件源对象
    /// </summary>
    public class OrderEventArgs
    {
        /// <summary>
        /// 订单IＤ
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 下单用户ＩＤ
        /// </summary>
        public int UserID { get; set; }
        /// <summary>
        /// 使用方式：１充值，２订单
        /// </summary>
        public UseType UseType { get; set; }
        /// <summary>
        /// 订单总金额
        /// </summary>
        public decimal TotalFee { get; set; }
        /// <summary>
        /// 订单项中的物品类型：0辅导班，1课件包
        /// </summary>
        public int SourceType { get; set; }
        /// <summary>
        /// 出现的异常字串
        /// </summary>
        public string ExceptionMessage { get; set; }
        /// <summary>
        /// 交易代码，这是第三方为我们返回的
        /// </summary>
        public string ExchangeCode { get; set; }

        public override string ToString()
        {
            return string.Format("orderid={0},userid={1},usertype={2},totalfee={3},sourcetype={4},ExchangeCode={5}"
                , OrderID
                , UserID
                , UserID
                , TotalFee
                , SourceType
                , ExchangeCode);
        }
    }
}
