using Lind.DDD.Logger;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;

namespace Lind.DDD.OnlinePay.WapAlipay
{
    /// <summary>
    /// wap支付宝实例
    /// </summary>
    public class Demo
    {
        WAPPay wapPay = new WAPPay();
        /// <summary>
        /// 发送支付请求
        /// </summary>
        public void Send()
        {
            wapPay.PayFormString("order001", "手机支付", 500);

        }
        /// <summary>
        /// 服务端回调（ＰＯＳＴ）
        /// </summary>
        public void Notify()
        {
            wapPay.AlipayNotify((orderId, tradeId) =>
            {
                //更新数据表相关逻辑
            });
        }
        /// <summary>
        /// 服务端回调（ＧＥＴ）
        /// </summary>
        public void Callback()
        {
            wapPay.AlipayCallback((orderId, tradeId) =>
            {
                //更新数据表相关逻辑
                //跳到成功页面
            }, (msg) =>
            {
                //跳到失败页面
            });
        }
    }
}
