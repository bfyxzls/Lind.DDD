using Lind.DDD.OnlinePay.Weixin.Params;
using Lind.DDD.OnlinePay.Weixin.Results;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.OnlinePay.Weixin
{
    public abstract class ClientBase
    {
        protected ParamBuilder ParamBuilder { get; set; }

        public ClientBase()
        {
            ParamBuilder = new ParamBuilder();
            SetParam("appid", PaymentConfig.Instance.AppId);
            SetParam("mch_id", PaymentConfig.Instance.Mch_Id);
            SetParam("device_info", PaymentConfig.Instance.Device_Info);
        }
        /// <summary>
        /// 设置参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value">int 或 string 类型</param>
        public void SetParam(string name, object value)
        {
            if(value is int)
                ParamBuilder.SetParam(name, Convert.ToInt32(value));
            else
                ParamBuilder.SetParam(name,Convert.ToString(value));
        }

        protected ResultBuilder PostForResult(string url, bool isCertificate, int timeOut = 10)
        {
            string xml = ParamBuilder.ToXml();
            string resultXml = PostToUrl(xml, url, false, timeOut);
            ResultBuilder resultBuilder = new ResultBuilder(resultXml);
            return resultBuilder;
        }

        protected string PostToUrl(string xml, string url, bool isCertificate, int timeOut = 10)
        {
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create(url);
            if (isCertificate)
            {
                string cert = PaymentConfig.Instance.CertPath;
                string password = PaymentConfig.Instance.CertPassword;
                X509Certificate cer = new X509Certificate(cert, password);
                webRequest.ClientCertificates.Add(cer);
            }
            webRequest.Method = "post";
            webRequest.Timeout = timeOut * 1000;
            
            if (!string.IsNullOrEmpty(xml))
            {
                webRequest.AllowAutoRedirect = true;
                //webRequest.ContentType = "application/x-www-form-urlencoded";
                //string strXML = "XMLDATA=" + xml;
                //byte[] data = Encoding.UTF8.GetBytes(strXML);

                webRequest.ContentType = "text/plain";
                byte[] data = Encoding.UTF8.GetBytes(xml);

                Stream newStream = webRequest.GetRequestStream();
                newStream.Write(data, 0, data.Length);
                newStream.Close();
            }
            HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
            string resultXml = GetResultXML(response);
            return resultXml;
        }

        protected string GetResultXML(HttpWebResponse response)
        {
            string value = null;

            // 获取响应流  
            using (Stream responseStream = response.GetResponseStream())
            {
                // 对接响应流(以"utf-8"字符集)  
                using (StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                {
                    // 开始读取数据  
                    value = sReader.ReadToEnd();
                }
            }

            return value;
        }

        protected string GetResultXml(HttpRequest request)
        {
            string value = null;

            // 获取响应流  
            using (Stream responseStream = request.InputStream)
            {
                // 对接响应流(以"utf-8"字符集)  
                using (StreamReader sReader = new StreamReader(responseStream, Encoding.GetEncoding("utf-8")))
                {
                    // 开始读取数据  
                    value = sReader.ReadToEnd();
                }
            }

            return value;
        }

        /// <summary>
        /// 验证参数
        /// </summary>
        protected virtual void ValidateParameters()
        {
            ValidateNotNullParam("appid", "公众账号ID");
            ValidateNotNullParam("mch_id", "商户号");
        }
        /// <summary>
        /// 验证一个参数是否为空
        /// </summary>
        /// <param name="name"></param>
        /// <param name="errorMessage"></param>
        protected void ValidateNotNullParam(string name,string errorMessage)
        {
            string value = ParamBuilder.GetParam(name);
            if (string.IsNullOrEmpty(name))
                throw new SDKRuntimeException(errorMessage + ":" + name);
        }

    }
}
