using Lind.DDD.OnlinePay.Weixin.Params;
using Lind.DDD.OnlinePay.Weixin.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.Weixin
{
    internal class UnifiedOrderClient : ClientBase
    {
        private string Url { get; set; }
        private int TimeOut { get; set; }
        public UnifiedOrderClient()
        {
            Url = PaymentConfig.Instance.UnifiedOrderUrl;
            TimeOut = PaymentConfig.Instance.TimeOut;
        }

        public UnifiedOrderResult PostXml(UnifiedOrderParam param)
        {
            SetParams(param);
            ValidateParameters();
            ResultBuilder resultBuilder = PostForResult(Url, false, TimeOut);
            resultBuilder.BasicValidate();
            UnifiedOrderResult result = new UnifiedOrderResult(resultBuilder);
            return result;
        }

        public string TestToShowXml(UnifiedOrderParam param)
        {
            SetParams(param);
            ValidateParameters();
            string xml = ParamBuilder.ToXml();
            return xml;
        }

        protected override void ValidateParameters()
        {
            base.ValidateParameters();

            ValidateNotNullParam("out_trade_no", "缺少统一支付接口必填参数");
            ValidateNotNullParam("body", "缺少统一支付接口必填参数");
            ValidateNotNullParam("total_fee", "缺少统一支付接口必填参数");
            ValidateNotNullParam("notify_url", "缺少统一支付接口必填参数");
            ValidateNotNullParam("trade_type", "缺少统一支付接口必填参数");

            if (ParamBuilder.GetParam("trade_type") == "JSAPI")
                ValidateNotNullParam("openid", "统一支付接口中，缺少必填参数openid！trade_type为JSAPI时，openid为必填参数！");
        }

        private void SetParams(UnifiedOrderParam param)
        {
            SetParam("body", param.Body);
            SetParam("attach", param.Attach);
            SetParam("out_trade_no", param.Out_trade_no);
            SetParam("total_fee", param.Total_fee);
            SetParam("spbill_create_ip", param.Spbill_create_ip);
            SetParam("time_start", param.Time_start);
            SetParam("time_expire", param.Time_expire);
            SetParam("goods_tag", param.Goods_tag);
            SetParam("notify_url", PaymentConfig.Instance.NOTIFY_URL);
            SetParam("trade_type", param.Trade_type);
            SetParam("openid", param.OpenId);
            SetParam("product_id", param.Product_id);
            SetParam("appId", param.AppId);

        }
    }

}
