using Lind.DDD.Logger;
using Lind.DDD.Utils.Encryptor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.OnlinePay.WebPay
{
    /// <summary>
    /// 支付宝支付功能实现
    /// </summary>
    public class AlipayImpl : PayBase
    {

        #region Override Properties
        /// <summary>
        /// 网关
        /// </summary>
        internal override string Gateway
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayGateway"] == null)
                    throw new ArgumentException("请配置AlipayGateway节点");
                return ConfigurationManager.AppSettings["AlipayGateway"];
            }
        }
        /// <summary>
        /// 合作伙伴
        /// </summary>
        internal override string Partner
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayPartner"] == null)
                    throw new ArgumentException("请配置AlipayPartner节点");
                return ConfigurationManager.AppSettings["AlipayPartner"];
            }
        }
        /// <summary>
        /// 支付宝回写地址
        /// </summary>
        internal override string ReturnAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayReturnUrl"] == null)
                    throw new ArgumentException("请配置AlipayReturnUrl节点");
                return ConfigurationManager.AppSettings["AlipayReturnUrl"];
            }
        }
        /// <summary>
        /// 支付宝回写地址(服务器回写)
        /// </summary>
        internal override string NotifyAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayNotifyUrl"] == null)
                    throw new ArgumentException("请配置AlipayNotifyUrl节点");
                return ConfigurationManager.AppSettings["AlipayNotifyUrl"];
            }
        }
        /// <summary>
        ///安全校验码 
        /// </summary>
        internal override string PublicKey
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayKey"] == null)
                    throw new ArgumentException("请配置AlipayKey节点");
                return ConfigurationManager.AppSettings["AlipayKey"];
            }
        }
        /// <summary>
        ///安全校验码 
        /// </summary>
        internal override string PrivateKey
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayKey"] == null)
                    throw new ArgumentException("请配置AlipayKey节点");
                return ConfigurationManager.AppSettings["AlipayKey"];
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// 服务标识
        /// </summary>
        internal string Service
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayService"] == null)
                    throw new ArgumentException("请配置AlipayService节点");
                return ConfigurationManager.AppSettings["AlipayService"];
            }
        }
        /// <summary>
        /// 编码类型
        /// </summary>
        internal string Input_Charset
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayInputCharSet"] == null)
                    throw new ArgumentException("请配置AlipayInputCharSet节点");
                return ConfigurationManager.AppSettings["AlipayInputCharSet"];
            }
        }
        /// <summary>
        /// 支付类型
        /// </summary>
        internal string Payment_Type
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayPaymentType"] == null)
                    throw new ArgumentException("请配置AlipayPaymentType节点");
                return ConfigurationManager.AppSettings["AlipayPaymentType"];
            }
        }
        /// <summary>
        /// 显示的URI
        /// </summary>
        internal string Show_Url
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayShowUrl"] == null)
                    throw new ArgumentException("请配置AlipayShowUrl节点");
                return ConfigurationManager.AppSettings["AlipayShowUrl"];
            }
        }
        /// <summary>
        /// 卖家账号
        /// </summary>
        internal string Seller_Email
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipaySellerEmail"] == null)
                    throw new ArgumentException("请配置AlipaySellerEmail节点");
                return ConfigurationManager.AppSettings["AlipaySellerEmail"];
            }
        }
        /// <summary>
        /// bankPay cartoon directPay
        /// </summary>
        internal string PayMethod
        {
            get
            {
                if (ConfigurationManager.AppSettings["AlipayPayMethod"] == null)
                    throw new ArgumentException("请配置AlipayPayMethod节点");
                return ConfigurationManager.AppSettings["AlipayPayMethod"];
            }
        }
        #endregion


        #region Server Send Methods
        protected override string GenerateUrl(UseType useType,
           string body,
           decimal vMoney,
           int userID,
           ref string exchangeID,
           string returnUrl,
           string notifyUrl,
           string name,
           string orderid = "")
        {
            Random random = new Random(System.Environment.TickCount);
            string out_trade_no = string.IsNullOrWhiteSpace(orderid) ? DateTime.Now.ToString("yyyyMMddHHmmss") + random.Next(10000, 99999) : orderid;
            exchangeID = out_trade_no;
            string total_fee = Math.Round(vMoney, 2).ToString();//金额 0.01～50000.00
            string[] para ={
                            "service="+Service,
                            "partner=" + Partner,
                            "seller_email=" + Seller_Email,
                            "out_trade_no=" + out_trade_no,//商家的订单号，支付宝只认这个商家号，在支付宝后台可以显示
                            "subject=" +  string.Format(base.GenerateSubject(useType),name),
                            "body=" + body,
                            "total_fee=" + total_fee, 
                            "show_url=" +Show_Url,
                            "payment_type=" + Payment_Type,
                            "paymethod=" + PayMethod,
                            "notify_url=" + notifyUrl,
                            "return_url=" + returnUrl,
                            "_input_charset=" + Input_Charset,
                            "defaultbank=ICBC"
                           };

            string aliay_url = CreatUrl(para, Input_Charset, PublicKey);

            string alipayStr = "";
            for (int i = 0; i < para.Length; i++)
                alipayStr += para[i] + "&";
            alipayStr += "sign=" + aliay_url;

            return Gateway + alipayStr;
        }

        /// <summary>
        /// 生成URL链接或加密结果
        /// </summary>
        /// <param name="para">参数加密数组</param>
        /// <param name="_input_charset">编码格式</param>
        /// <param name="key">安全校验码</param>
        /// <returns>字符串URL或加密结果</returns>
        private string CreatUrl(string[] para, string _input_charset, string key)
        {
            int i;
            //进行排序；
            string[] Sortedstr = BubbleSort(para);
            //构造待md5摘要字符串 ；
            StringBuilder prestr = new StringBuilder();
            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (i == Sortedstr.Length - 1)
                {
                    prestr.Append(Sortedstr[i]);
                }
                else
                {
                    prestr.Append(Sortedstr[i] + "&");
                }
            }
            prestr.Append(key);
            //生成Md5摘要；
            string sign = GetMD5(prestr.ToString(), _input_charset);
            //以下是POST方式传递参数
            return sign;
        }

        #endregion

        #region Server Return Methods

        /// <summary>
        /// 支付宝Notify & Return回写操作
        /// Notify使用Post方式，Return使用Get方法并完成URL的重定向
        /// </summary>
        /// <param name="httpMethod">Http请求方式，0为Get，1为Post</param>
        public override void Notify_Return(HttpMethod httpMethod)
        {

            var Request = HttpContext.Current.Request;
            var Response = HttpContext.Current.Response;
            var get_post = httpMethod == HttpMethod.Get ? Request.QueryString : Request.Form;
            string body = Utility.DecryptString(Request.Params["body"].ToString());
            int userID = Convert.ToInt32(body.Split('|')[0]);
            string orderID = "0";
            string orderIDs = string.Empty;
            orderID = body.Split('|')[1].Split(',')[0];
            if (body.Split('|')[1].Split(',').Length > 1)
            {
                orderIDs = Utility.DecryptString(body.Split('|')[1]);
            }
            int types = Convert.ToInt32(body.Split('|')[2]);
            decimal totalFee = Convert.ToDecimal(body.Split('|')[3]);
            int sourceType = Convert.ToInt32(body.Split('|')[4]);
            string exchangeID = Request.Params["trade_no"].ToString();//支付宝的交易号
            string exception = null;
            bool ret = false;

            try
            {
                StringBuilder result = new StringBuilder();
                foreach (var item in get_post.AllKeys)
                {
                    result.Append(item + get_post[item].ToString() + ",");
                }
                LoggerFactory.Instance.Logger_Info("支付宝回调参数集合：" + result.ToString());
                ret = AlipayReturnParam(userID,
                                        orderID,
                                        exchangeID,
                                        get_post["notify_id"],
                                        get_post["sign"],
                                        get_post,
                                        System.Web.HttpContext.Current);
            }
            catch (Exception ex)
            {
                exception = "sign信息验证状态：" + ret + "，异常信息：" + ex.Message;
                LoggerFactory.Instance.Logger_Info(exception);

            }

            var model = new OrderEventArgs
            {
                OrderID = orderID,//商家订单号
                UserID = userID,
                UseType = (UseType)types,
                TotalFee = totalFee,
                SourceType = sourceType,
                ExceptionMessage = exception,
                ExchangeCode = exchangeID//支付宝交易号
            };
            try
            {
                LoggerFactory.Instance.Logger_Info(model.ToString());
            }
            catch (Exception ex)
            {

                LoggerFactory.Instance.Logger_Info(ex.Message);
            }

            if (ret)
            {
                try
                {
                    //成功
                    OnSuccess(model);
                    if (httpMethod == HttpMethod.Post)
                        Response.Write("success");
                }
                catch (Exception)//业务层出现问题，向支付宝也输出fail
                {
                    Response.Write("fail");
                }


            }
            else
            {
                try
                {
                    OnFail(model);
                    if (httpMethod == HttpMethod.Post)
                        Response.Write("fail");
                }
                catch (Exception)
                {
                    Response.Write("fail");
                }

            }
        }

        /// <summary>
        /// 支付宝Return回写参数
        /// </summary>
        /// <param name="userID"></param>
        /// <param name="orderID"></param>
        /// <param name="rpID"></param>
        /// <param name="exchangeID"></param>
        /// <param name="notify_id"></param>
        /// <param name="sign"></param>
        /// <param name="coll"></param>
        /// <param name="http"></param>
        /// <returns></returns>
        bool AlipayReturnParam(
          int userID,
          string orderID,
          string exchangeID,
          string notify_id,
          string sign,
          NameValueCollection coll,
          System.Web.HttpContext http)
        {

            string alipayNotifyURL = "http://notify.alipay.com/trade/notify_query.do?";
            string partner = ConfigurationManager.AppSettings["AlipayPartner"].ToString();
            string key = ConfigurationManager.AppSettings["AlipayKey"].ToString();
            string _input_charset = ConfigurationManager.AppSettings["AlipayInputCharSet"].ToString();
            alipayNotifyURL = alipayNotifyURL + "&partner=" + partner + "&notify_id=" + notify_id;
            string responseTxt = AliPay.Get_Http(alipayNotifyURL, 120000);

            int i;
            String[] requestarr = coll.AllKeys;

            //进行排序；
            string[] Sortedstr = AliPay.BubbleSort(requestarr);

            //构造待md5摘要字符串 ；
            StringBuilder prestr = new StringBuilder();

            for (i = 0; i < Sortedstr.Length; i++)
            {
                if (coll[Sortedstr[i]] != "" &&
                    Sortedstr[i] != "sign" &&
                    Sortedstr[i] != "sign_type")
                {
                    if (i == Sortedstr.Length - 1)
                    {
                        prestr.Append(Sortedstr[i] + "=" + coll[Sortedstr[i]]);
                    }
                    else
                    {
                        prestr.Append(Sortedstr[i] + "=" + coll[Sortedstr[i]] + "&");
                    }
                }
            }

            prestr.Append(key);

            //生成Md5摘要；
            string mysign = AliPay.GetMD5(prestr.ToString(), _input_charset);
            LoggerFactory.Instance.Logger_Info(string.Format("mysin={0},sign={1},responseTxt={2}", mysign, sign, responseTxt));
            if (mysign == sign && responseTxt == "true")   //验证支付发过来的消息，签名是否正确
            {
                //日志，成功
                return true;
            }
            else
            {
                //日志，失败
                return false;
            }
        }
        #endregion

    }
}
