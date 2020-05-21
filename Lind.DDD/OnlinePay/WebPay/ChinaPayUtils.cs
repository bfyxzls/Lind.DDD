using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lind.DDD.OnlinePay.WebPay
{
    /// <summary>
    /// 银联辅助类
    /// </summary>
    public class ChinaPayUtils
    {
        public static string priKeyPath = "";
        public static string pubKeyPath = "";
        public ChinaPayUtils()
        {
        }

        //2007签名
        public static string signData(string merId, string plain)
        {
            byte[] StrRes = Encoding.GetEncoding("utf-8").GetBytes(plain);
            plain = Convert.ToBase64String(StrRes);

            //NetPayClientClass a = new NetPayClientClass();
            NetPay a = new NetPay();
            //设置密钥文件地址
            a.buildKey(merId, 0, priKeyPath);

            // 对一段字符串的签名
            return a.Sign(plain);

        }

        //2004签名
        public static string sign(string MerId, string OrdId, string TransAmt, string CuryId, string TransDate, string TransType)
        {

            //NetPayClientClass a = new NetPayClientClass();
            NetPay a = new NetPay();
            //设置密钥文件地址
            a.buildKey(MerId, 0, priKeyPath);

            // 对一段字符串的签名
            return a.signOrder(MerId, OrdId, TransAmt, CuryId, TransDate, TransType);

        }

        //验签
        public static string checkData(string plain, string ChkValue)
        {
            byte[] StrRes = Encoding.GetEncoding("utf-8").GetBytes(plain);
            plain = Convert.ToBase64String(StrRes);

            NetPay a = new NetPay();
            //设置密钥文件地址
            a.buildKey("999999999999999", 0, pubKeyPath);

            // 对一段字符串的签名
            if (a.verifyAuthToken(plain, ChkValue))
            {
                return "0";
            }
            else
            {
                return "-118";
            }



        }


        //得到交易日期
        public static string getMerDate()
        {
            return DateTime.Now.ToString("yyyyMMdd");
        }

        //得到订单号16位
        public static string getMerSeqId()
        {
            return DateTime.Now.ToString("yyyyMMHHmmffffff");
        }

        //16位退款号
        public static string getMerRefId()
        {
            return DateTime.Now.ToString("yyyyMMHHmmffffff");
        }

        public static string getBase64(string str)
        {
            byte[] StrRes = Encoding.GetEncoding("utf-8").GetBytes(str);
            return Convert.ToBase64String(StrRes);
        }

        /// <summary> 
        /// 取得客户端真实IP。如果有代理则取第一个非内网地址（能处理多层代理的问题） 
        /// <![CDATA[其原理可查看：《C#取真实IP地址及分析》]]>
        /// </summary> 
        public static string getRealIPAddress
        {
            get
            {
                string result = String.Empty;

                result = System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (result != null && result != String.Empty)
                {
                    //可能有代理 
                    if (result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式 
                        result = null;
                    else
                    {
                        if (result.IndexOf(",") != -1)
                        {
                            //有“,”，估计多个代理。取第一个不是内网的IP。 
                            result = result.Replace(" ", "").Replace("'", "");
                            string[] temparyip = result.Split(",;".ToCharArray());
                            for (int i = 0; i < temparyip.Length; i++)
                            {
                                if (IsIPAddress(temparyip[i])
                                    && temparyip[i].Substring(0, 3) != "10."
                                    && temparyip[i].Substring(0, 7) != "192.168"
                                    && temparyip[i].Substring(0, 7) != "172.16.")
                                {
                                    return temparyip[i];    //找到不是内网的地址 
                                }
                            }
                        }
                        else if (IsIPAddress(result)) //代理即是IP格式 
                            return result;
                        else
                            result = null;    //代理中的内容 非IP，取IP 
                    }

                }

                string IpAddress = (System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != null && System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] != String.Empty) ? System.Web.HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] : System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (null == result || result == String.Empty)
                    result = System.Web.HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];

                if (result == null || result == String.Empty)
                    result = System.Web.HttpContext.Current.Request.UserHostAddress;

                return result;
            }
        }

        /// <summary>
        /// 判断是否是IP地址格式 0.0.0.0
        /// </summary>
        /// <param name="str1">待判断的IP地址</param>
        /// <returns>true or false</returns>
        static bool IsIPAddress(string str)
        {
            if (str == null || str == string.Empty || str.Length < 7 || str.Length > 15) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";


            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str);
        }

    }

}
