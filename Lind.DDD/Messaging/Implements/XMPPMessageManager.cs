using Lind.DDD.Logger;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Messaging.Implements
{
    public class XMPPMessageManager : IMessageManager
    {
        #region Singleton
        private static object lockObj = new object();
        public static XMPPMessageManager Instance;
        static XMPPMessageManager()
        {
            lock (lockObj)
            {
                if (Instance == null)
                    Instance = new XMPPMessageManager();
            }
        }
        private XMPPMessageManager()
        { }
        #endregion

        public readonly string WEBREQUSET_ADDRESS = System.Configuration.ConfigurationManager.AppSettings["website_apprequest"] ?? "http://192.168.2.7:9090/";//移动API请求地址
        public readonly string WEBREQUSET_SWITCH = System.Configuration.ConfigurationManager.AppSettings["switch_apprequest"] ?? "true";//移动API请求地址


        public int Send(string recipient, string subject, string body)
        {
            return Send(recipient, subject, body, null, null);
        }

        public int Send(string recipient, string subject, string body, Action errorAction = null, Action successAction = null)
        {
            bool result = false;
            bool.TryParse(WEBREQUSET_SWITCH, out result);
            if (result)
            {
                try
                {
                    string url = WEBREQUSET_ADDRESS;
                    HttpWebRequest objWebRequest = (HttpWebRequest)WebRequest.Create(url);
                    objWebRequest.Method = "POST";
                    objWebRequest.ContentType = "application/x-www-form-urlencoded";
                    byte[] SomeBytes = System.Text.Encoding.UTF8.GetBytes("form=0&to=" + recipient + "&" + body);
                    objWebRequest.ContentLength = SomeBytes.Length;
                    Stream stream = objWebRequest.GetRequestStream();
                    stream.Write(SomeBytes, 0, SomeBytes.Length);
                    stream.Close();
                    HttpWebResponse response = (HttpWebResponse)objWebRequest.GetResponse();
                    Stream s = response.GetResponseStream();
                    StreamReader reader = new StreamReader(s, System.Text.Encoding.UTF8);
                    string respHTML = reader.ReadToEnd();
                    response.Close();
                    return 1;
                }
                catch (Exception ex)
                {
                    LoggerFactory.Instance.Logger_Info(ex.Message);
                    return 0;
                }

            }
            return 0;
        }

        public int Send(IEnumerable<string> recipients, string subject, string body, Action errorAction = null, Action successAction = null)
        {
            try
            {
                foreach (var recipient in recipients)
                {
                    Send(recipient, subject, body, successAction, errorAction);
                }
                return 1;
            }
            catch (Exception)
            {
                return 0;
            }

        }
    }
}
