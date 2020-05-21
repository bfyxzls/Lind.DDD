using Lind.DDD.Logger;
using Lind.DDD.Utils;
using Lind.DDD.Utils.Encryptor;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Lind.DDD.OnlinePay.WebPay
{
    public class ChinaPayImpl : PayBase
    {
        #region Override Properties
        /// <summary>
        /// 支付接入地址
        /// </summary>
        internal override string Gateway
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayAction"] == null)
                    throw new ArgumentException("请配置ChinaPayAction节点");
                return ConfigurationManager.AppSettings["ChinaPayAction"].ToString();
            }
        }
        /// <summary>
        /// 商户号，必填
        /// </summary>
        internal override string Partner
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayMerId"] == null)
                    throw new ArgumentException("请配置ChinaPayMerId节点");
                return ConfigurationManager.AppSettings["ChinaPayMerId"].ToString();
            }
        }
        internal override string ReturnAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayPageRetUrl"] == null)
                    throw new ArgumentException("请配置ChinaPayPageRetUrl节点");
                return ConfigurationManager.AppSettings["ChinaPayPageRetUrl"].ToString();
            }
        }
        internal override string NotifyAddress
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayBgRetUrl"] == null)
                    throw new ArgumentException("请配置ChinaPayBgRetUrl节点");
                return ConfigurationManager.AppSettings["ChinaPayBgRetUrl"].ToString();
            }
        }
        internal override string PublicKey
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayPubKeyPath"] == null)
                    throw new ArgumentException("请配置ChinaPayPubKeyPath节点");
                return ConfigurationManager.AppSettings["ChinaPayPubKeyPath"].ToString();
            }
        }
        internal override string PrivateKey
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayPriKeyPath"] == null)
                    throw new ArgumentException("请配置ChinaPayPriKeyPath节点");
                return ConfigurationManager.AppSettings["ChinaPayPriKeyPath"].ToString();
            }
        }
        #endregion

        #region Properties
        /// <summary>
        /// //支付网关号，选填
        /// </summary>
        internal string ChinaPayGateId
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayGateId"] == null)
                    throw new ArgumentException("请配置ChinaPayGateId节点");
                return ConfigurationManager.AppSettings["ChinaPayGateId"].ToString();
            }
        }
        /// <summary>
        /// 业务编号，选填
        /// </summary>
        internal string ChinaPayBusiId
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayBusiId"] == null)
                    throw new ArgumentException("请配置ChinaPayBusiId节点");
                return ConfigurationManager.AppSettings["ChinaPayBusiId"].ToString();
            }
        }
        /// <summary>
        /// 订单币种，必填
        /// </summary>
        internal string ChinaPayCuryId
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayCuryId"] == null)
                    throw new ArgumentException("请配置ChinaPayCuryId节点");
                return ConfigurationManager.AppSettings["ChinaPayCuryId"].ToString();
            }
        }
        /// <summary>
        /// 支付接入版本号，必填
        /// </summary>
        internal string ChinaPayVersion
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayVersion"] == null)
                    throw new ArgumentException("请配置ChinaPayVersion节点");
                return ConfigurationManager.AppSettings["ChinaPayVersion"].ToString();
            }
        }
        /// <summary>
        /// 分账类型，必填
        /// </summary>
        internal string ChinaPayShareType
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayShareType"] == null)
                    throw new ArgumentException("请配置ChinaPayShareType节点");
                return ConfigurationManager.AppSettings["ChinaPayShareType"].ToString();
            }
        }
        /// <summary>
        /// 商户分账数据，必填
        /// </summary>
        internal string ChinaPayShareData
        {
            get
            {
                if (ConfigurationManager.AppSettings["ChinaPayShareData"] == null)
                    throw new ArgumentException("请配置ChinaPayShareData节点");
                return ConfigurationManager.AppSettings["ChinaPayShareData"].ToString();
            }
        }
        #endregion

        #region Server Send Methods
        protected override string GenerateUrl(UseType useType, string body, decimal vMoney, int userID, ref string exchangeID, string returnUrl, string notifyUrl, string name, string orderid = "")
        {
            Random random = new Random(System.Environment.TickCount);
            string out_trade_no = DateTime.Now.ToString("yyyyMMdd") + random.Next(10, 99);
            exchangeID = out_trade_no;
            string OrdId = out_trade_no;
            string OrdAmt = (Convert.ToInt32(Convert.ToDecimal(vMoney) * 100)).ToString();//订单金额，单位：分，必填
            string OrdDesc = string.Format(base.GenerateSubject(useType), name);
            string Priv1 = body;//商户自填信息
            string CustomIp = IPHelper.getRealIPAddress;//用户提交订单的IP地址，选填
            string ChkValue = "";//签名数据，必填
            ChinaPayUtils.priKeyPath = HttpContext.Current.Server.MapPath("/" + ConfigurationManager.AppSettings["ChinaPayPriKeyPath"].ToString());
            string plainData = Partner
               + ChinaPayBusiId
               + OrdId
               + OrdAmt
               + ChinaPayCuryId
               + ChinaPayVersion
               + NotifyAddress
               + ReturnAddress
               + ChinaPayGateId
               + ChinaPayShareType
               + ChinaPayShareData + OrdAmt
               + Priv1
               + CustomIp;
            ChkValue = ChinaPayUtils.signData(Partner, plainData);

            return Gateway + "?"
                           + "MerId=" + Partner
                           + "&BusiId=" + ChinaPayBusiId
                           + "&OrdId=" + OrdId
                           + "&OrdAmt=" + OrdAmt
                           + "&CuryId=" + ChinaPayCuryId
                           + "&Version=" + ChinaPayVersion
                           + "&BgRetUrl=" + NotifyAddress
                           + "&PageRetUrl=" + ReturnAddress
                           + "&GateId=" + ChinaPayGateId
                           + "&OrdDesc=" + OrdDesc
                           + "&ShareType=" + ChinaPayShareType
                           + "&ShareData=" + ChinaPayShareData + OrdAmt
                           + "&Priv1=" + Priv1
                           + "&CustomIp=" + CustomIp
                           + "&ChkValue=" + ChkValue;
        }
        #endregion

        #region Server Return Methods
        public override void Notify_Return(HttpMethod httpMethod)
        {
            #region 赋值
            var Response = HttpContext.Current.Response;

            string body = Utility.DecryptString(Get_Post_Return("Priv1"), Utility.EncryptorType.DES);
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
            string exchangeID = Get_Post_Return("OrdId");
            bool ret = false;
            #endregion

            try
            {

                ret = ChinaPayRetUrlParam(
                Get_Post_Return("MerId"),
                Get_Post_Return("BusiId"),
                Get_Post_Return("OrdId"),
                Get_Post_Return("OrdAmt"),
                Get_Post_Return("CuryId"),
                Get_Post_Return("Version"),
                Get_Post_Return("GateId"),
                Get_Post_Return("OrdDesc"),
                Get_Post_Return("ShareType"),
                Get_Post_Return("ShareData"),
                Get_Post_Return("Priv1"),
                Get_Post_Return("CustomIp"),
                Get_Post_Return("PayStat"),
                Get_Post_Return("PayTime"),
                Get_Post_Return("ChkValue"));
            }
            catch (Exception ex)
            {
                LoggerFactory.Instance.Logger_Info("sign信息验证状态：" + ret + "，异常信息：" + ex.Message);
            }
            LoggerFactory.Instance.Logger_Info(body);
            LoggerFactory.Instance.Logger_Info(Get_Post_Return("ChkValue"));

            var model = new OrderEventArgs
            {
                OrderID = orderID,
                UserID = userID,
                UseType = (UseType)types,
                TotalFee = totalFee / 100,//将分转换为元
                SourceType = sourceType,
                ExchangeCode = "银联交易状态：" + Get_Post_Return("PayStat") + ",交易时间：" + Get_Post_Return("PayTime"),
                

            };
            if (ret)
            {
                //成功
                OnSuccess(model);
                Response.Write("success");
            }
            else
            {
                //成功
                OnFail(model);
                Response.Write("success");
            }

        }

        /// <summary>
        /// 银联Notify回写参数
        /// </summary>
        /// <param name="MerId"></param>
        /// <param name="BusiId"></param>
        /// <param name="OrdId"></param>
        /// <param name="OrdAmt"></param>
        /// <param name="CuryId"></param>
        /// <param name="Versions"></param>
        /// <param name="BgRetUrl"></param>
        /// <param name="PageRetUrl"></param>
        /// <param name="GateId"></param>
        /// <param name="OrdDesc"></param>
        /// <param name="ShareType"></param>
        /// <param name="ShareData"></param>
        /// <param name="Priv1"></param>
        /// <param name="CustomIp"></param>
        /// <param name="PayStat">银联為我們返回的，防治篡改的</param>
        /// <param name="PayTime">银联為我們返回的，防治篡改的</param>
        /// <param name="ChkValue"></param>
        /// <returns></returns>
        public bool ChinaPayRetUrlParam(
            string MerId,
            string BusiId,
            string OrdId,
            string OrdAmt,
            string CuryId,
            string Versions,
            string GateId,
            string OrdDesc,
            string ShareType,
            string ShareData,
            string Priv1,
            string CustomIp,
            string PayStat,
            string PayTime,
            string ChkValue)
        {
            ChinaPayUtils.pubKeyPath = HttpContext.Current.Server.MapPath("/" + ConfigurationManager.AppSettings["ChinaPayPubKeyPath"].ToString());
            string plainData = MerId
                + BusiId
                + OrdId
                + OrdAmt
                + CuryId
                + Versions
                + GateId
                + ShareType
                + ShareData
                + Priv1
                + CustomIp
                + PayStat
                + PayTime;
            string i = ChinaPayUtils.checkData(plainData, ChkValue);
            LoggerFactory.Instance.Logger_Info("plainData:" + plainData);
            LoggerFactory.Instance.Logger_Info("ChkValue:" + ChkValue);
            LoggerFactory.Instance.Logger_Info("plainData&ChkValue:" + i);
            if (PayStat == "1001" && i == "0")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

    }



}
