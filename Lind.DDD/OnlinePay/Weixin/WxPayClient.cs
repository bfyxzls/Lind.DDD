using Lind.DDD.OnlinePay.Weixin.Params;
using Lind.DDD.OnlinePay.Weixin.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin
{
    public class WxPayClient
    {
        /// <summary>
        /// 创建支付订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public UnifiedOrderResult CreateUnifiedOrder(UnifiedOrderParam param)
        {
            UnifiedOrderClient unifiedOrder = new UnifiedOrderClient();
            return unifiedOrder.PostXml(param);
        }

        /// <summary>
        /// 接收消息通知
        /// </summary>
        /// <param name="onNotified"></param>
        public void Notified(Func<NotifyResult, NotifyReturnMessage> onNotified)
        {
            NotifyClient notifyClient = new NotifyClient();
            notifyClient.Notified(onNotified);
        }

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public OrderQueryResult QueryOrder(OrderQueryParam param)
        {
            OrderQueryClient orderQueryClient = new OrderQueryClient();
            return orderQueryClient.PostXml(param);
        }

    }
}
