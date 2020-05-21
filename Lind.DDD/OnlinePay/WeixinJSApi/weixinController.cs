using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Mvc;

namespace Lind.DDD.OnlinePay.WeixinJSApi
{
    /// <summary>
    /// 需要实现微信支付的控制器可以继承它
    /// </summary>
    public class weixinController : System.Web.Mvc.Controller
    {
        #region 普通模式
        /// <summary>
        /// 单击支付按钮，斛发的动作
        /// </summary>
        /// <param name="orderId">当前订单号</param>
        /// <param name="money">价格</param>
        /// <param name="openId">主页上产生的openId</param>
        /// <returns></returns>
        public string Get(string orderId, int money, string openId)
        {
            Logger.LoggerFactory.Instance.Logger_Info("发送订单号" + orderId);
            //JsApiImplement.GetOpenId();加到主页面上
            return JsApiImplement.Send(money, orderId, openId);
        }

        /// <summary>
        /// 第三方子网站通过这个方法拿到当前的code
        /// </summary>
        /// <param name="appid"></param>
        /// <param name="returnUrl"></param>
        public void GetCode(string appid)
        {
            JsApiImplement.GetCode(appid);
        }
        /// <summary>
        /// 支付页面:如何去拿到OpenId
        /// </summary>
        /// <returns></returns>
        public ActionResult PayPage()
        {
            ViewBag.OpenId = JsApiImplement.GetOpenId();
            return View();
        }
        #endregion

        #region 代理模式
        /// <summary>
        /// 微信支付
        /// 从代理服务器获取openID
        /// 注意：代理服务器与本服务器需要有预定，双方要告诉对应的URL地址，用来进行二次跳转
        /// </summary>
        /// <returns></returns>
        public ActionResult Weixin()
        {
            string openId = Request.QueryString["openId"];
            if (string.IsNullOrWhiteSpace(openId))
            {
                //第一次加载页面，跳转到代理服务器获openid
                Logger.LoggerFactory.Instance.Logger_Info("openid为空");
                Response.Redirect("http://m.158.cn/order/GetOpenId");

            }
            else
            {
                //由代理服务器跳回来后，得到本用户的openId
                Logger.LoggerFactory.Instance.Logger_Info("从m158拿到openid:" + openId);
                ViewBag.openId = openId;
            }
            return View();
        }

        /// <summary>
        /// 微信支付的代理服务
        /// </summary>
        /// <returns></returns>
        public void WeixinProxy()
        {
            string openId = JsApiImplement.GetOpenId();//获取当前openId
            string bussinessUrl = "";//业务网站支付地址
            if (!string.IsNullOrWhiteSpace(openId))
                Redirect(bussinessUrl + "?openId=" + openId);
        }
        /// <summary>
        /// 获取支付JSON串
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="money"></param>
        /// <param name="openId"></param>
        /// <returns></returns>
        public string GetPayJson(string orderId, int money, string openId)
        {
            try
            {
                Logger.LoggerFactory.Instance.Logger_Info("发送订单号=" + orderId + ",openID=" + openId);
                //JsApiImplement.GetOpenId();加到主页面上
                return JsApiImplement.Send(money, orderId, openId);

            }
            catch (Exception ex)
            {

                Logger.LoggerFactory.Instance.Logger_Error(ex);
                return "出错了";
            }

        }
        #endregion

        //微信回调
        public void Notify()
        {
            JsApiImplement.Notify((model) =>
            {
                Logger.LoggerFactory.Instance.Logger_Info("回调订单号" + model.Out_Trade_No);
                if (model.IsSuccess)
                {
                    //微信回调成功
                    //更新领域订单状态
                }
                else
                {
                    //微信回调失败
                }


            });
        }
    }
}