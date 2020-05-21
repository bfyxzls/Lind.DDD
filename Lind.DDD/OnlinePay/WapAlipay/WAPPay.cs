using Lind.DDD.Logger;
using Lind.DDD.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using System.Xml;

namespace Lind.DDD.OnlinePay.WapAlipay
{
    /// <summary>
    /// 支付宝回调方法委托
    /// </summary>
    /// <param name="out_trade_no">商户订单号</param>
    /// <param name="trade_no">支付宝交易号</param>
    /// <param name="message">消息</param>
    /// <returns></returns>
    public delegate void AlipayCallBack(string out_trade_no, string trade_no);
    /// <summary>
    /// 业务参数
    /// </summary>
    public class AliPayBussiness
    {
        private string bodys;
        private string subjects;
        private string outtradeno;
        private string timeoutexpress;
        private string totalmounts;
        private string seller_Id;
        private string product_Code;
        private string goods_Type;
        private string passback_Params;
        private string promo_Params;
        private string extend_Params;
        private string enable_pay_Channels;
        private string disable_pay_Channels;


        /// <summary>
        /// 对一笔交易的具体描述信息。如果是多种商品，请将商品描述字符串累加传给body。
        /// [非必填项]
        /// </summary>
        public string body
        {
            get { return bodys; }
            set { bodys = value; }
        }

        /// <summary>
        /// 商品的标题/交易标题/订单标题/订单关键字等
        /// [必填项]
        /// </summary>
        public string subject
        {
            get { return subjects; }
            set { subjects = value; }
        }

        /// <summary>
        /// 商户网站唯一订单号
        /// [必填项]
        /// </summary>
        public string out_trade_no
        {
            get { return outtradeno; }
            set { outtradeno = value; }
        }


        /*
         该笔订单允许的最晚付款时间，逾期将关闭交易。
         * 取值范围：1m～15d。m-分钟，h-小时，d-天，1c-当天（1c-当天的情况下，无论交易何时创建，都在0点关闭）。
         * 该参数数值不接受小数点， 如 1.5h，可转换为 90m。
         */
        /// <summary>
        /// 最晚付款时间
        /// [非必填项]
        /// </summary>
        public string timeout_express
        {
            get { return timeoutexpress; }
            set { timeoutexpress = value; }
        }

        /// <summary>
        /// 订单总金额，单位为元，精确到小数点后两位，取值范围[0.01,100000000]
        /// [必填项]
        /// </summary>
        public string total_amount
        {
            get { return totalmounts; }
            set { totalmounts = value; }
        }

        /// <summary>
        /// 销售产品码，商家和支付宝签约的产品码，为固定值QUICK_MSECURITY_PAY
        /// [必填项]
        /// </summary>
        public string product_code
        {
            get { return product_Code != null ? product_Code : "QUICK_MSECURITY_PAY"; }
            set { product_Code = value; }
        }


        /// <summary>
        /// 收款支付宝用户ID。 如果该值为空，则默认为商户签约账号对应的支付宝用户ID
        /// [非必填项]
        /// </summary>
        public string seller_id
        {
            get { return seller_Id; }
            set { seller_Id = value; }
        }
    }
    /// <summary>
    /// wap站支付
    /// </summary>
    public class WAPPay
    {
        //卖家支付宝帐户
        private string seller_email;
        //服务器异步通知页面路径
        string notify_url;
        //页面跳转同步通知页面路径
        private string call_back_url;
        //操作中断返回地址
        private string merchant_url;

        private HttpRequest Request;
        private HttpResponse Response;

        public WAPPay()
        {
            seller_email = WebConfig.GetWebConfig("AlipaySellerEmail", "");
            notify_url = WebConfig.GetWebConfig("AlipayWAPNotifyUrl", "");
            call_back_url = WebConfig.GetWebConfig("AlipayWAPCallbackUrl", "");
            merchant_url = WebConfig.GetWebConfig("AlipayWAPMerchantUrl", "");

            Config.Partner = WebConfig.GetWebConfig("AlipayPartner", "");
            Config.Private_key = WebConfig.GetWebConfig("AlipayWAPPrivateKey", "");
            Config.Public_key = WebConfig.GetWebConfig("AlipayWAPPublicKey", "");

            Request = HttpContext.Current.Request;
            Response = HttpContext.Current.Response;
        }

        /// <summary>
        /// H5发送请求到Alipay Wap
        /// </summary>
        /// <param name="orderId">商户订单号,不传直接用当前日期时间</param>
        /// <param name="subject">订单名称</param>
        /// <param name="total_fee">付款金额</param>
        /// <returns></returns>
        public string PayFormString(string orderId = null, string subject = null, decimal total_fee = 0m)
        {
            //支付宝网关地址
            string GATEWAY_NEW = System.Configuration.ConfigurationManager.AppSettings["WapAlipayGateway"] ?? "http://wappaygw.alipay.com/service/rest.htm?";

            ////////////////////////////////////////////调用授权接口alipay.wap.trade.create.direct获取授权码token////////////////////////////////////////////

            //返回格式
            string format = "xml";
            //必填，不需要修改

            //返回格式
            string v = "2.0";
            //必填，不需要修改
            Random random = new Random(System.Environment.TickCount);
            //请求号
            string req_id = string.IsNullOrWhiteSpace(orderId) ? DateTime.Now.ToString("yyyyMMddHHmmss") + random.Next(10000, 99999) : orderId;
            //请求业务参数详细
            string req_dataToken = "<direct_trade_create_req><notify_url>" + notify_url + "</notify_url><call_back_url>" + call_back_url + "</call_back_url><seller_account_name>" + seller_email + "</seller_account_name><out_trade_no>" + orderId + "</out_trade_no><subject>" + subject + "</subject><total_fee>" + total_fee + "</total_fee><merchant_url>" + merchant_url + "</merchant_url></direct_trade_create_req>";
            //必填

            //把请求参数打包成数组
            Dictionary<string, string> sParaTempToken = new Dictionary<string, string>();
            sParaTempToken.Add("partner", Config.Partner);
            sParaTempToken.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTempToken.Add("sec_id", Config.Sign_type.ToUpper());
            sParaTempToken.Add("service", "alipay.wap.trade.create.direct");
            sParaTempToken.Add("format", format);
            sParaTempToken.Add("v", v);
            sParaTempToken.Add("req_id", req_id);
            sParaTempToken.Add("req_data", req_dataToken);


            //建立请求
            string sHtmlTextToken = Submit.BuildRequest(GATEWAY_NEW, sParaTempToken);
            //URLDECODE返回的信息
            Encoding code = Encoding.GetEncoding(Config.Input_charset);
            sHtmlTextToken = HttpUtility.UrlDecode(sHtmlTextToken, code);

            //解析远程模拟提交后返回的信息
            Dictionary<string, string> dicHtmlTextToken = Submit.ParseResponse(sHtmlTextToken);

            //获取token
            string request_token = dicHtmlTextToken["request_token"];

            ////////////////////////////////////////////根据授权码token调用交易接口alipay.wap.auth.authAndExecute////////////////////////////////////////////


            //业务详细
            string req_data = "<auth_and_execute_req><request_token>" + request_token + "</request_token></auth_and_execute_req>";
            //必填

            //把请求参数打包成数组
            Dictionary<string, string> sParaTemp = new Dictionary<string, string>();
            sParaTemp.Add("partner", Config.Partner);
            sParaTemp.Add("_input_charset", Config.Input_charset.ToLower());
            sParaTemp.Add("sec_id", Config.Sign_type.ToUpper());
            sParaTemp.Add("service", "alipay.wap.auth.authAndExecute");
            sParaTemp.Add("format", format);
            sParaTemp.Add("v", v);
            sParaTemp.Add("req_data", req_data);

            //建立请求
            string sHtmlText = Submit.BuildRequest(GATEWAY_NEW, sParaTemp, "get", "确认");
            StringBuilder str = new StringBuilder();
            foreach (var item in sParaTemp.Keys)
                str.Append(item + "=" + sParaTemp[item] + ",");
            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("WAP向支付宝发出请求，发送的参数：" + str.ToString());

            return sHtmlText;
        }

        /// <summary>
        /// 直接为APP返回的支付字符串
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="subject"></param>
        /// <param name="total_fee"></param>
        /// <returns></returns>
        public string AppPayFormString(string orderNo, decimal totalFee, string subject = null, string body = "Lind")
        {
            //必填，不需要修改
            Random random = new Random(System.Environment.TickCount);
            //请求号
            string req_id = string.IsNullOrWhiteSpace(orderNo) ? DateTime.Now.ToString("yyyyMMddHHmmss") + random.Next(10000, 99999) : orderNo;

            SortedDictionary<string, object> sParaTemp = new SortedDictionary<string, object>();

            AliPayBussiness bussin = new AliPayBussiness()
            {
                timeout_express = "10m",
                seller_id = Config.AlipayConfig.Partner,
                product_code = "QUICK_MSECURITY_PAY",
                total_amount = totalFee.ToString("0.00"),
                subject = subject,
                body = body,
                out_trade_no = orderNo,
            };
            sParaTemp.Add("app_id", Config.AlipayConfig.AppId);

            string bizcont = SerializeMemoryHelper.SerializeToJson<AliPayBussiness>(bussin);
            sParaTemp.Add("biz_content", bizcont);
            sParaTemp.Add("charset", Config.Input_charset);
            sParaTemp.Add("method", "alipay.trade.app.pay");
            sParaTemp.Add("format", "json");
            sParaTemp.Add("sign_type", Config.AlipayConfig.Sign_type); //这个单拿出来,和sign拼到最后
            sParaTemp.Add("timestamp", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            sParaTemp.Add("version", "1.0");
            sParaTemp.Add("notify_url", Config.AlipayConfig.Notify_url);
            //生成Url
            string content = sParaTemp.ToUrl();
            //生成签名
            string sign = RSAFromPkcs8.sign(content, Config.AlipayConfig.Private_key, Config.AlipayConfig.Input_charset);
            //追加签名
            content += "&sign=" + sign;
            //整个字符做UrlEncode
            content = RSAFromPkcs8.EncodeEx(content);
            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("向APP返回的字符串包括Sign:" + content);
            return content;
        }


        /// <summary>
        /// 支付宝回调地址POST
        /// </summary>
        /// <param name="notifySuccess">回调成功后的逻辑,商户订单号和支付宝交易号回调到调用端</param>
        public void AlipayNotify(AlipayCallBack notifySuccess)
        {
            string msg = "";
            Dictionary<string, string> sPara = GetRequestPost();
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.VerifyNotify(sPara, Request.Form["sign"]);

                if (verifyResult)//验证成功
                {
                    //解密（如果是RSA签名需要解密，如果是MD5签名则下面一行清注释掉）
                    sPara = aliNotify.Decrypt(sPara);
                    //XML解析notify_data数据
                    try
                    {
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(sPara["notify_data"]);
                        //商户订单号(我们生成的，最好使用日期时间的表式方法，用于成功回调后，修改订单状态)
                        string out_trade_no = xmlDoc.SelectSingleNode("/notify/out_trade_no").InnerText;
                        //支付宝交易号(支付宝生成的)
                        string trade_no = xmlDoc.SelectSingleNode("/notify/trade_no").InnerText;
                        //交易状态
                        string trade_status = xmlDoc.SelectSingleNode("/notify/trade_status").InnerText;

                        if (trade_status == "TRADE_FINISHED")
                        {
                            //判断该笔订单是否在商户网站中已经做过处理
                            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                            //如果有做过处理，不执行商户的业务程序

                            //注意：
                            //该种交易状态只在两种情况下出现
                            //1、开通了普通即时到账，买家付款成功后。
                            //2、开通了高级即时到账，从该笔交易成功时间算起，过了签约时的可退款时限（如：三个月以内可退款、一年以内可退款等）后。
                            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("WAP支付宝回调成功，订单号：" + out_trade_no + "，交易号：" + trade_no);

                            notifySuccess(out_trade_no, trade_no);
                            Response.Write("success");
                        }
                        else if (trade_status == "TRADE_SUCCESS")
                        {
                            //判断该笔订单是否在商户网站中已经做过处理
                            //如果没有做过处理，根据订单号（out_trade_no）在商户网站的订单系统中查到该笔订单的详细，并执行商户的业务程序
                            //如果有做过处理，不执行商户的业务程序
                            //注意：
                            //该种交易状态只在一种情况下出现——开通了高级即时到账，买家付款成功后。
                            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("WAP支付宝回调成功，订单号：" + out_trade_no + "，交易号：" + trade_no);
                            StringBuilder str = new StringBuilder();
                            foreach (var item in sPara.Keys)
                                str.Append(item + "=" + sPara[item] + ",");
                            Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info(str.ToString());
                            notifySuccess(out_trade_no, trade_no);
                            Response.Write("success");
                        }
                        else
                        {
                            Response.Write("fail");
                            LoggerFactory.Instance.Logger_Info(string.Format("WAP支付失败，订单号：{0},状态：{1}", out_trade_no, msg));
                        }

                    }
                    catch (Exception exc)
                    {
                        Response.Write("fail");
                        LoggerFactory.Instance.Logger_Info(string.Format("WAP支付失败，异常信息:{0}", exc.Message));
                    }
                }
                else//验证失败
                {
                    LoggerFactory.Instance.Logger_Info(string.Format("WAP验证失败"));

                    Response.Write("fail");
                }
            }
            else
            {
                Response.Write("fail");
            }

        }
        /// <summary>
        ///  支付宝回调GET
        /// </summary>
        /// <param name="notifySuccess">支付成功的回调，商户订单号和支付宝交易号回调到调用端</param>
        /// <param name="notifyFail">支付失败的回调</param>
        public void AlipayCallback(AlipayCallBack notifySuccess, Action<string> notifyFail)
        {
            Dictionary<string, string> sPara = GetRequestGet();
            if (sPara.Count > 0)//判断是否有带返回参数
            {
                Notify aliNotify = new Notify();
                bool verifyResult = aliNotify.VerifyReturn(sPara, Request.QueryString["sign"]);

                if (verifyResult)//验证成功
                {
                    //商户订单号
                    string out_trade_no = Request.QueryString["out_trade_no"];
                    //支付宝交易号
                    string trade_no = Request.QueryString["trade_no"];
                    //交易状态
                    string result = Request.QueryString["result"];
                    Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("WAP支付宝回调成功，订单号：" + out_trade_no + "，交易号：" + trade_no);
                    StringBuilder str = new StringBuilder();
                    foreach (var item in sPara.Keys)
                        str.Append(item + "=" + sPara[item] + ",");
                    Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info(str.ToString());
                    notifySuccess(out_trade_no, trade_no);
                }
                else//验证失败
                {
                    LoggerFactory.Instance.Logger_Info(string.Format("WAP验证失败"));
                    notifyFail("验证失败");
                }
            }
            else
            {
                LoggerFactory.Instance.Logger_Info(string.Format("WAP无返回参数"));
                notifyFail("无返回参数");
            }
        }

        /// <summary> 
        /// 获取支付宝GET过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private Dictionary<string, string> GetRequestGet()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.QueryString;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.QueryString[requestItem[i]]);
            }

            return sArray;
        }

        /// <summary>
        /// 获取支付宝POST过来通知消息，并以“参数名=参数值”的形式组成数组
        /// </summary>
        /// <returns>request回来的信息组成的数组</returns>
        private Dictionary<string, string> GetRequestPost()
        {
            int i = 0;
            Dictionary<string, string> sArray = new Dictionary<string, string>();
            NameValueCollection coll;
            //Load Form variables into NameValueCollection variable.
            coll = Request.Form;

            // Get names of all forms into a string array.
            String[] requestItem = coll.AllKeys;

            for (i = 0; i < requestItem.Length; i++)
            {
                sArray.Add(requestItem[i], Request.Form[requestItem[i]]);
            }

            return sArray;
        }

    }
}