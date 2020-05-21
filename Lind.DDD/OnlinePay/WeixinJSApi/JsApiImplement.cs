using Lind.DDD.OnlinePay.WeixinJSApi.business;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.OnlinePay.WeixinJSApi
{
    /// <summary>
    /// 构建支付处理类
    /// </summary>
    public class JsApiImplement
    {
        /// <summary>
        /// H5调起JS API参数
        /// </summary>
        public static string wxJsApiParam { get; set; }
        /// <summary>
        /// 返回当前微信客户端的OpenId，每个客户端在每个公众号里的OpenId是唯一的
        /// </summary>
        /// <returns></returns>
        public static string GetOpenId()
        {
            JsApiPay jsApiPay = new JsApiPay(System.Web.HttpContext.Current);
            jsApiPay.GetOpenidAndAccessToken();
            Log.Debug("GetOpenId", "openid : " + jsApiPay.openid);
            return jsApiPay.openid;
        }
        /// <summary>
        /// 第三方子网站通过这个方法拿到当前的code
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="returnUrl"></param>
        public static void GetCode(string appid)
        {
            JsApiPay jsApiPay = new JsApiPay(System.Web.HttpContext.Current);
            jsApiPay.GetCodeByAppId(appid);
        }
        /// <summary>
        /// JsApi返回微信支付的连接参数，这个方法需要前台UI页面调用，通常可以使用AJAX进行调用它
        /// </summary>
        /// <param name="total_fee">订单金额</param>
        /// <param name="orderId">业务的订单编号</param>
        /// <returns></returns>
        public static string Send(int total_fee, string orderId, string openId)
        {
            try
            {
                //调用【网页授权获取用户信息】接口获取用户的openid和access_token
                //jsApiPay.GetOpenidAndAccessToken();
                JsApiPay jsApiPay = new JsApiPay(System.Web.HttpContext.Current);
                jsApiPay.openid = openId;
                Log.Debug("Send", "openid : " + jsApiPay.openid);
                //若传递了相关参数，则调统一下单接口，获得后续相关接口的入口参数,微信的价格是分
                jsApiPay.total_fee = total_fee;
                WxPayData unifiedOrderResult = jsApiPay.GetUnifiedOrderResult(orderId);
                wxJsApiParam = jsApiPay.GetJsApiParameters();//获取H5调起JS API参数                    
                Log.Debug("Send", "wxJsApiParam : " + wxJsApiParam);
            }
            catch (Exception ex)
            {
                Log.Error("Error", ex.Message);
            }
            return wxJsApiParam;
        }
        /// <summary>
        /// JsApi微信回调
        /// </summary>
        public static void Notify(Action<NotifyModel> action)
        {
            try
            {
                Log.Info("Notify", "微信Notify被回调");
                var context = System.Web.HttpContext.Current;
                ResultNotify resultNotify = new ResultNotify(context);
                resultNotify.ProcessNotify(action);
            }
            catch (Exception ex)
            {
                Log.Error("Error", ex.Message);
            }

        }
    }
}