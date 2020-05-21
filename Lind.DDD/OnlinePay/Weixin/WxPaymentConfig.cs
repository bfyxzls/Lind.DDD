using Lind.DDD.CacheConfigFile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lind.DDD.OnlinePay.Weixin
{
    /// <summary>
    /// 配置信息类，存储在当前网站目录configs下面
    /// </summary>
    public class WxPaymentConfig : PaymentConfig, IConfiger
    {
        /// <summary>
        /// 发给微信的订单号的前缀，用于区分内测，外测和外正
        /// </summary>
        public string OutTradeNoPrefix { get; set; }
    }
}