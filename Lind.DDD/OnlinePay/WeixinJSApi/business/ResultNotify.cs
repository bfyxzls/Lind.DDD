using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Lind.DDD.OnlinePay.WeixinJSApi.business
{
    /// <summary>
    /// 支付结果通知回调处理类
    /// 负责接收微信支付后台发送的支付结果并对订单有效性进行验证，将验证结果反馈给微信支付后台
    /// </summary>
    public class ResultNotify : Notify
    {

        Action<string> action;
        public ResultNotify(HttpContext page)
            : base(page)
        {
        }

        public override void ProcessNotify(Action<NotifyModel> action)
        {
            WxPayData notifyData = GetNotifyData();

            //检查支付结果中transaction_id是否存在
            if (!notifyData.IsSet("transaction_id"))
            {
                //若transaction_id不存在，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "支付结果中微信订单号不存在");
                Log.Error(this.GetType().ToString(), "The Pay result is error : " + res.ToXml());
                action(new NotifyModel
                {
                    IsSuccess = false,
                    NotifyMessage = "支付结果中微信订单号不存在",
                });
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
            string transaction_id = notifyData.GetValue("transaction_id").ToString();
            string out_trade_no = notifyData.GetValue("out_trade_no").ToString();
            string openId = notifyData.GetValue("openid").ToString();
            string uniqueId = string.Empty;//new JsApiPay(System.Web.HttpContext.Current).GetUnionIdByOpenId(openId);
            //查询订单，判断订单真实性
            if (!QueryOrder(transaction_id))
            {
                //若订单查询失败，则立即返回结果给微信支付后台
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "FAIL");
                res.SetValue("return_msg", "订单查询失败");
                Log.Error(this.GetType().ToString(), "Order query failure : " + res.ToXml());
                action(new NotifyModel
                {
                    IsSuccess = false,
                    NotifyMessage = "订单查询失败",
                    Transaction_Id = transaction_id,
                    Out_Trade_No = out_trade_no,
                    OpenId = openId,
                    UniqueId = uniqueId
                });
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
            //查询订单成功
            else
            {
                WxPayData res = new WxPayData();
                res.SetValue("return_code", "SUCCESS");
                res.SetValue("return_msg", "OK");
                Log.Info(this.GetType().ToString(), "order query success : " + res.ToXml());
                //处理相关业务逻辑
                NotifyModel model = new NotifyModel
                {
                    Transaction_Id = transaction_id,
                    Out_Trade_No = out_trade_no,
                    OpenId = openId,
                    UniqueId = uniqueId,
                    IsSuccess = true,
                    NotifyMessage = "回调成功",
                };
                action(model);
                page.Response.Write(res.ToXml());
                page.Response.End();
            }
        }

        //查询订单
        private bool QueryOrder(string transaction_id)
        {
            WxPayData req = new WxPayData();
            req.SetValue("transaction_id", transaction_id);
            WxPayData res = WxPayApi.OrderQuery(req);
            if (res.GetValue("return_code").ToString() == "SUCCESS" &&
                res.GetValue("result_code").ToString() == "SUCCESS")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}