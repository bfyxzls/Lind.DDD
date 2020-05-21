using Lind.DDD.CacheConfigFile;
using Lind.DDD.OnlinePay.Weixin.Params;
using Lind.DDD.OnlinePay.Weixin.Results;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;


namespace Lind.DDD.OnlinePay.Weixin
{
    /// <summary>
    /// 微信支付实现者
    /// </summary>
    public class WxPayImpl
    {
        #region Public Methods
        /// <summary>
        /// 提交到微信,返回二维码的ＵＲＬ，通过二维码工具解析它
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="vMoney">单位：分</param>
        /// <param name="userID">用户ID</param>
        /// <param name="product_id">产品ID</param>
        /// <returns></returns>
        public string RechargeTo(string orderid, decimal vMoney, int userID, string product_id = "")
        {
            var totalfee = (int)vMoney;
            string strImaUrl = string.Empty;
            Dictionary<string, string> dictReturnParams = new Dictionary<string, string>();
            try
            {

                WxPayClient client = GetWxPayClient();
                UnifiedOrderParam unifiedOrderParam = new UnifiedOrderParam()
                  {
                      Body = "微信支付",
                      Out_trade_no = orderid,
                      Total_fee = totalfee,
                      Spbill_create_ip = "",
                      Time_start = DateTime.Now.ToString("yyyyMMddHHmmss"),
                      Time_expire = DateTime.Now.AddMinutes(30).ToString("yyyyMMddHHmmss"),
                      Trade_type = TradeType.NATIVE,
                      Product_id = product_id
                  };
                UnifiedOrderResult result = client.CreateUnifiedOrder(unifiedOrderParam);
                if (result.Success)
                {
                    dictReturnParams.Add("CodeUrl", result.Code_Url);
                    dictReturnParams.Add("PrepayId", result.Prepay_Id);
                    strImaUrl = result.Code_Url;
                }
                else
                {
                    dictReturnParams.Add("Error", result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                dictReturnParams.Add("Error", ex.Message);
            }
            return strImaUrl;
        }
        /// <summary>
        /// 微信的回调
        /// </summary>
        /// <param name="callBack">成功的回调，参数为交易ID，由服务端发到支付宝，再由支付宝返回</param>
        /// <param name="errBack">失败的回调，参数为失败的消息</param>
        public void RecieveWxPayNotify(Action<NotifyResult> callBack, Action<NotifyResult> errBack)
        {
            try
            {
                WxPayClient client = GetWxPayClient();
                client.Notified((result) =>
                {
                    NotifyReturnMessage message = new NotifyReturnMessage()
                    {
                        Success = true,
                        Message = "收到",
                    };
                    try
                    {
                        if (result.Success)
                        {
                            string rechargeID = result.Out_Trade_No;
                            string sPath = System.Web.HttpContext.Current.Request.MapPath("/");
                            //----以下是处理微信付款后回调服务处理微信订单入本地库
                            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("微信回调成功，订单号：" + rechargeID + ",交易号：" + result.Transaction_Id);
                            callBack(result);
                        }
                        else
                        {
                            string msg = string.Format("收到微信通知的错误消息：{0}({1})", result.ErrorMessage, result.ErrorCode);
                            errBack(result);
                            Logger.LoggerFactory.Instance.Logger_Info(msg);
                        }
                    }
                    catch (Exception ex)
                    {
                        message.Success = false;
                        message.Message = ex.Message;
                        Logger.LoggerFactory.Instance.Logger_Error(ex);
                    }
                    return message;
                });
            }
            catch (Exception ex)
            {
                Logger.LoggerFactory.Instance.Logger_Error(ex);
            }
        }

        #endregion

        #region   Methods

        WxPayClient GetWxPayClient()
        {
            WxPaymentConfig config = GetWxPaymentConfig();
            PaymentConfig.Init(config);
            WxPayClient wxPayClient = new WxPayClient();
            return wxPayClient;
        }

        /// <summary>
        /// 构建微信配置文件
        /// </summary>
        /// <param name="path">自定义路径，为空表示/configs/WxPaymentConfig.config</param>
        /// <returns></returns>
        WxPaymentConfig GetWxPaymentConfig()
        {
            return ConfigFactory.Instance.GetConfig<WxPaymentConfig>();
        }

        #endregion

    }
}