using Lind.DDD.OnlinePay.Weixin.Params;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin
{
    public class TestWxPayClient : WxPayClient
    {
        public TestWxPayClient(PaymentConfig config)
        {
            PaymentConfig.Init(config);
        }

        public string TestToShowOrderRequestXml(UnifiedOrderParam param)
        {
            UnifiedOrderClient unifiedOrder = new UnifiedOrderClient();
            return unifiedOrder.TestToShowXml(param);
        }
    }
}
