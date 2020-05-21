using Lind.DDD.Utils.Encryptor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.OnlinePay.WebPay
{
    /// <summary>
    /// 在线支付委托
    /// </summary>
    /// <param name="e">数据源</param>
    public delegate void OnlineEventHandler(OrderEventArgs e);

    /// <summary>
    /// 第三方支付基类
    /// </summary>
    public abstract class PayBase
    {
        #region Events
        /// <summary>
        /// 在线支付成功
        /// </summary>
        public event OnlineEventHandler Success;
        /// <summary>
        /// 在线支付失败
        /// </summary>
        public event OnlineEventHandler Fail;
        /// <summary>
        /// 成功时触发
        /// </summary>
        /// <param name="e"></param>
        protected void OnSuccess(OrderEventArgs e)
        {
            if (Success != null)
                Success(e);
        }
        /// <summary>
        /// 失败时触发
        /// </summary>
        /// <param name="e"></param>
        protected void OnFail(OrderEventArgs e)
        {
            if (Fail != null)
                Fail(e);
        }
        #endregion

        #region Properties
        /// <summary>
        /// 域名
        /// </summary>
        internal string DoMain
        {
            get
            {
                if (ConfigurationManager.AppSettings["DoMain"] == null)
                    throw new ArgumentException("请配置DoMain节点");
                return ConfigurationManager.AppSettings["DoMain"].ToString();
            }
        }
        /// <summary>
        /// 网站名称
        /// </summary>
        internal string SiteName
        {
            get
            {
                if (ConfigurationManager.AppSettings["SiteName"] == null)
                    throw new ArgumentException("请配置SiteName节点");
                return ConfigurationManager.AppSettings["SiteName"].ToString();
            }
        }
        #endregion

        #region Abstract Properties
        /// <summary>
        /// 网关
        /// </summary>
        internal abstract string Gateway { get; }
        /// <summary>
        /// 合作伙伴
        /// </summary>
        internal abstract string Partner { get; }
        /// <summary>
        /// 第三方回写地址
        /// </summary>
        internal abstract string ReturnAddress { get; }
        /// <summary>
        ///  第三方回写地址（服务器自动调用）
        /// </summary>
        internal abstract string NotifyAddress { get; }
        /// <summary>
        ///安全校验码 （公钥）
        /// </summary>
        internal abstract string PublicKey { get; }
        /// <summary>
        /// 安全校验码 （私钥）
        /// </summary>
        internal abstract string PrivateKey { get; }

        #endregion

        #region Methods,Facade Pattern

        /// <summary>
        /// 第三方充值对外接口
        /// </summary>
        /// <param name="vMoney"></param>
        /// <param name="userID"></param>
        /// <param name="rUserID"></param>
        /// <param name="types"></param>
        /// <returns></returns>
        public void RechargeTo(decimal vMoney, int userID)
        {
            string exchangeID = "";
            /**
              body:{userid}|{orderid}|{type}|{totalFee}
             */
            string body = Utility.EncryptString(string.Format("{0}|{1}|{2}|{3}|{4}"
                , userID.ToString()
                , 0
                , UseType.Recharge.GetHashCode()
                , vMoney, 0));
            string urlString = GenerateUrl(UseType.Recharge, body, vMoney, userID, ref exchangeID, ReturnAddress, NotifyAddress, string.Empty);
            HttpContext.Current.Response.Redirect(urlString);
        }
        /// <summary>
        /// 第三方支付对外接口
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="useType"></param>
        /// <param name="userID"></param>
        /// <param name="money"></param>
        /// <param name="productname"></param>
        public void OnlineTo(string orderid, int sourceType, int userID, decimal money, string productname, string tradeId = "")
        {


            int _orderid = 0;
            string exchangeID = string.Empty;
            if (orderid.IndexOf(',') > -1)
            {
                int.TryParse(orderid.Split(new char[] { ',' })[1], out _orderid);
            }
            else
            {
                int.TryParse(orderid, out _orderid);
            }
            /**
              body:{userid}|{orderid}|{type}|{totalFee}
             */
            string body = Utility.EncryptString(string.Format("{0}|{1}|{2}|{3}|{4}"
                , userID.ToString()
                , orderid
                , UseType.Order.GetHashCode()
                , money
                , sourceType));
            HttpContext.Current.Response.Redirect(GenerateUrl(UseType.Order, body, money, userID, ref exchangeID, ReturnAddress, NotifyAddress, productname, tradeId));
        }
        /// <summary>
        /// 根据使用方法得到订单商品名称
        /// </summary>
        /// <param name="useType"></param>
        /// <returns></returns>
        protected string GenerateSubject(UseType useType)
        {
            switch (useType)
            {
                case UseType.Recharge:
                    return "{0}";
                case UseType.Order:
                    return "{0}";
                default:
                    throw new ArgumentException("参数错误");
            }
        }
        /// <summary>
        /// 冒泡排序法
        /// 按照字母序列从a到z的顺序排列
        /// </summary>
        protected string[] BubbleSort(string[] r)
        {
            int i, j; //交换标志 
            string temp;

            bool exchange;

            for (i = 0; i < r.Length; i++) //最多做R.Length-1趟排序 
            {
                exchange = false; //本趟排序开始前，交换标志应为假

                for (j = r.Length - 2; j >= i; j--)
                {//交换条件
                    if (System.String.CompareOrdinal(r[j + 1], r[j]) < 0)
                    {
                        temp = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = temp;

                        exchange = true; //发生了交换，故将交换标志置为真 
                    }
                }

                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
            return r;
        }
        /// <summary>
        /// 与ASP兼容的MD5加密算法
        /// </summary>
        protected string GetMD5(string s, string _input_charset)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding(_input_charset).GetBytes(s));
            StringBuilder sb = new StringBuilder(32);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }
            return sb.ToString();
        }
        /// <summary>
        /// 返回Get或者Post键所对应的值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected string Get_Post_Return(string key)
        {
            var Request = HttpContext.Current.Request;
            return (Request.QueryString[key] ?? Request.Form[key]).ToString();
        }
        #endregion

        #region Abstract Methods，Every subClass implement.
        /// <summary>
        /// 服务器通知与返回
        /// </summary>
        /// <param name="httpMethod">0为Get,1为Post</param>
        public abstract void Notify_Return(HttpMethod httpMethod);
        /// <summary>
        /// 生成第三方支付的URI，由具体类去实现
        /// </summary>
        /// <param name="useType">使用目的</param>
        /// <param name="body"></param>
        /// <param name="vMoney">订单金额</param>
        /// <param name="userID">买家ＩＤ</param>
        /// <param name="exchangeID"></param>
        /// <param name="returnUrl"></param>
        /// <param name="notifyUrl"></param>
        /// <param name="name"></param>
        /// <param name="orderid">前台可能传过来的订单号</param>
        /// <returns></returns>
        protected abstract string GenerateUrl(UseType useType, string body, decimal vMoney, int userID, ref string exchangeID, string returnUrl, string notifyUrl, string name, string orderid = "");
        #endregion


    }
}
