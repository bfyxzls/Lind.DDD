using Lind.DDD.OnlinePay.Weixin.Params;
using Lind.DDD.OnlinePay.Weixin.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 
namespace Lind.DDD.OnlinePay.Weixin
{
    internal class OrderQueryClient : ClientBase
    {
        public OrderQueryResult PostXml(OrderQueryParam param)
        {
            SetParams(param);
            ValidateParameters();
            ResultBuilder resultBuilder = PostForResult(PaymentConfig.Instance.OrderQueryUrl, false, PaymentConfig.Instance.TimeOut);
            //结果验证
            resultBuilder.BasicValidate();
            resultBuilder.ValidateNotNullField("trade_state", "交易状态为空");

            OrderQueryResult result = new OrderQueryResult(resultBuilder);

            return result;
        }

        protected override void ValidateParameters()
        {
            base.ValidateParameters();

            ValidateNotNullParam("out_trade_no", "缺少商户系统内部的订单号必填参数");
        }

        private void SetParams(OrderQueryParam param)
        {
            SetParam("transaction_id", param.Transaction_Id);
            SetParam("out_trade_no", param.Out_Trade_No);
        }

    }
}
