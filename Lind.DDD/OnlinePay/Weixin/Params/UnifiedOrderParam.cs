using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin.Params
{
    /// <summary>
    /// 统一支付接口参数
    /// 自定义参数写在Attach里
    /// 注册本参数不支持扩展，因为校验是在微信服务端完成的
    /// </summary>
    public class UnifiedOrderParam
    {
        /// <summary>
        /// 商品描述
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 附加数据  (非必填)
        /// </summary>
        public string Attach { get; set; }
        /// <summary>
        /// 商户订单号
        /// </summary>
        public string Out_trade_no { get; set; }
        /// <summary>
        /// 总金额 单位为分
        /// </summary>
        public int Total_fee { get; set; }
        /// <summary>
        /// 终端IP  订单生成的机器IP
        /// </summary>
        public string Spbill_create_ip { get; set; }
        /// <summary>
        /// 交易起始时间  yyyyMMddHHmmss  (非必填)
        /// </summary>
        public string Time_start { get; set; }
        /// <summary>
        /// 交易结束时间  yyyyMMddHHmmss  (非必填)
        /// </summary>
        public string Time_expire { get; set; }
        /// <summary>
        /// 商品标记  不能随便填，不使用请置空  (非必填)
        /// </summary>
        public string Goods_tag { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public TradeType Trade_type { get; set; }
        /// <summary>
        /// 用户标识   (非必填)  JSAPI模式时必填
        /// </summary>
        public string OpenId { get; set; }
        /// <summary>
        /// 商品ID   (非必填)  NATIVE模式时必填
        /// </summary>
        public string Product_id { get; set; }
        /// <summary>
        /// 公众号，商家ID
        /// </summary>
        public string AppId { get; set; }
    }
    

}
