using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;

namespace Lind.DDD.Utils
{
    public class IPHelper
    {
        /// <summary>
        /// 得到ＩＰ地址
        /// </summary>
        /// <returns></returns>
        public static string GetRealIPAddress()
        {
            return GetRealIPAddress(null);
        }
        public static string GetRealIPAddress(NameValueCollection serverVariables)
        {
            string result = String.Empty;
            try
            {
                serverVariables = serverVariables ?? System.Web.HttpContext.Current.Request.ServerVariables;
                result = serverVariables["HTTP_X_FORWARDED_FOR"];

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
                                if (IPHelper.IsIPAddress(temparyip[i])
                                    && temparyip[i].Substring(0, 3) != "10."
                                    && temparyip[i].Substring(0, 7) != "192.168"
                                    && temparyip[i].Substring(0, 7) != "172.16.")
                                {
                                    return temparyip[i];    //找到不是内网的地址 
                                }
                            }
                        }
                        else if (IPHelper.IsIPAddress(result)) //代理即是IP格式 
                            return result;
                        else
                            result = null;    //代理中的内容 非IP，取IP 
                    }

                }

                string IpAddress = (serverVariables["HTTP_X_FORWARDED_FOR"] != null
                    && serverVariables["HTTP_X_FORWARDED_FOR"] != String.Empty)
                    ? serverVariables["HTTP_X_FORWARDED_FOR"]
                    : serverVariables["REMOTE_ADDR"];

                if (null == result || result == String.Empty)
                    result = serverVariables["REMOTE_ADDR"];
            }
            catch (Exception)
            {

            }

            return result;
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
                                if (IPHelper.IsIPAddress(temparyip[i])
                                    && temparyip[i].Substring(0, 3) != "10."
                                    && temparyip[i].Substring(0, 7) != "192.168"
                                    && temparyip[i].Substring(0, 7) != "172.16.")
                                {
                                    return temparyip[i];    //找到不是内网的地址 
                                }
                            }
                        }
                        else if (IPHelper.IsIPAddress(result)) //代理即是IP格式 
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
        public static bool IsIPAddress(string str)
        {
            if (str == null || str == string.Empty || str.Length < 7 || str.Length > 15) return false;
            string regformat = @"^\d{1,3}[\.]\d{1,3}[\.]\d{1,3}[\.]\d{1,3}$";


            Regex regex = new Regex(regformat, RegexOptions.IgnoreCase);
            return regex.IsMatch(str);
        }

        /// <summary>
        /// 将IP转换为十进制形式
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static ulong IP2Number(string ip)
        {
            byte[] bytes = System.Net.IPAddress.Parse(ip.Trim()).GetAddressBytes();
            ulong ret = 0;
            foreach (byte b in bytes)
            {
                ret <<= 8;
                ret |= b;
            }
            return ret;
        }

    }
}
