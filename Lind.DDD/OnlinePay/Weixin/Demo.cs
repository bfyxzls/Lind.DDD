using Lind.DDD.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.OnlinePay.Weixin
{
    /// <summary>
    /// 微信支付的调用
    /// </summary>
    public class Demo
    {
        WxPayImpl wxPayImpl = new WxPayImpl();
        /// <summary>
        /// 发送支付请求
        /// </summary>
        public void Send()
        {
            WxPayImpl wxPayImpl = new WxPayImpl();
            HttpResponseBase Response = null;
            QRCodeHelper.OutPutQRCodeImage(wxPayImpl.RechargeTo("order001", 500, 1, "product"), Response);
        }

        /// <summary>
        /// 输出图片,可以在支付页面单独生成图片，一般使用<image src='/weixin/image'></image>进行承载
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public void Image(string code)
        {
            HttpResponseBase Response = null;
            QRCodeHelper.OutPutQRCodeImage(code, Response);

            //$.ajax({
            //  url: "/Order/index",
            //  data: { subject: $("#subject").val(), price: $("#price").val() },
            //  type: "POST",
            //  success: function (data) {
            //      $("#codeImage").attr("src", "/Order/Image?code=" + data);
            //      $("#codeImage").show();
            //  }
            //});
        }

        public void Notify()
        {
            wxPayImpl.RecieveWxPayNotify((result) =>
            {
                //微信处理成功后的逻辑
            }, (result) => { });
        }
    }
}
