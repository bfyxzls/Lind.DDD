using System.Web;
using System.Text;
using System.IO;
using System.Net;
using System;
using System.Collections.Generic;
using Lind.DDD.CacheConfigFile;

namespace Lind.DDD.OnlinePay.WapAlipay
{
    /// <summary>
    /// 类名：Config
    /// 功能：基础配置类
    /// 详细：设置帐户有关信息及返回路径
    /// 版本：3.3
    /// 日期：2012-07-05
    /// 说明：
    /// 以下代码只是为了方便商户测试而提供的样例代码，商户可以根据自己网站的需要，按照技术文档编写,并非一定要使用该代码。
    /// 该代码仅供学习和研究支付宝接口使用，只是提供一个参考。
    /// 
    /// 如何获取安全校验码和合作身份者ID
    /// 1.用您的签约支付宝账号登录支付宝网站(www.alipay.com)
    /// 2.点击“商家服务”(https://b.alipay.com/order/myOrder.htm)
    /// 3.点击“查询合作者身份(PID)”、“查询安全校验码(Key)”
    /// </summary>
    public class Config : IConfiger
    {
        public static AlipayConfig AlipayConfig = CacheConfigFile.ConfigFactory.Instance.GetConfig<AlipayConfig>();
        #region 字段
        private static string partner = "";
        private static string key = "";
        private static string private_key = "";
        private static string public_key = "";
        private static string input_charset = "";
        private static string sign_type = "";
        private static string notify_url = "";
        #endregion

        static Config()
        {
            //↓↓↓↓↓↓↓↓↓↓请在这里配置您的基本信息↓↓↓↓↓↓↓↓↓↓↓↓↓↓↓

            //合作身份者ID，以2088开头由16位纯数字组成的字符串
            partner = "";

            //交易安全检验码，由数字和字母组成的32位字符串
            //如果签名方式设置为“MD5”时，请设置该参数
            key = "";

            //商户的私钥
            //如果签名方式设置为“0001”时，请设置该参数
            private_key = @"";

            //支付宝的公钥
            //如果签名方式设置为“0001”时，请设置该参数
            public_key = @"";

            //↑↑↑↑↑↑↑↑↑↑请在这里配置您的基本信息↑↑↑↑↑↑↑↑↑↑↑↑↑↑↑



            //字符编码格式 目前支持 utf-8
            input_charset = "utf-8";

            //签名方式，选择项：0001(RSA)、MD5
            sign_type = "0001";
            //无线的产品中，签名方式为rsa时，sign_type需赋值为0001而不是RSA
        }

        #region 属性
        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public static string Partner
        {
            get { return partner; }
            set { partner = value; }
        }

        /// <summary>
        /// 获取或设交易安全校验码
        /// </summary>
        public static string Key
        {
            get { return key; }
            set { key = value; }
        }

        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public static string Private_key
        {
            get { return private_key; }
            set { private_key = value; }
        }

        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public static string Public_key
        {
            get { return public_key; }
            set { public_key = value; }
        }

        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public static string Input_charset
        {
            get { return input_charset; }
        }

        /// <summary>
        /// 获取签名方式
        /// </summary>
        public static string Sign_type
        {
            get { return sign_type; }
        }

        /// <summary>
        /// 支付宝服务器主动通知商户服务器里指定的页面http/https路径。
        /// </summary>
        public static string Notify_url
        {
            get { return notify_url; }
            set { notify_url = value; }
        }
        #endregion
    }


    public class AlipayConfig : IConfiger
    {
        public AlipayConfig()
        {
            this.AppId = "支付宝应用ID";
            this.Partner = "2088开头";
            this.Key = "";
            this.Private_key = "私钥1024或者2048个字符";
            this.Public_key = "公钥,支付宝那边也会存";
            this.Sign_type = "RSA";
            this.Notify_url = "支付宝回调你平台的路径,需要是外网地址";
            this.Input_charset = "utf-8";
        }

        #region 属性

        /// <summary>
        /// 应用Id
        /// </summary>
        public string AppId { get; set; }
        /// <summary>
        /// 获取或设置合作者身份ID
        /// </summary>
        public string Partner { get; set; }
        /// <summary>
        /// 获取或设交易安全校验码
        /// </summary>
        public string Key { get; set; }


        /// <summary>
        /// 获取或设置商户的私钥
        /// </summary>
        public string Private_key { get; set; }


        /// <summary>
        /// 获取或设置支付宝的公钥
        /// </summary>
        public string Public_key { get; set; }



        /// <summary>
        /// 获取字符编码格式
        /// </summary>
        public string Input_charset { get; set; }


        /// <summary>
        /// 获取签名方式
        /// </summary>
        public string Sign_type { get; set; }

        /// <summary>
        /// 支付宝服务器主动通知商户服务器里指定的页面http/https路径。
        /// </summary>
        public string Notify_url { get; set; }

        #endregion
    }

}