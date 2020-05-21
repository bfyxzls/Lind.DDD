using Lind.DDD.Logger;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lind.DDD.Messaging.Implements
{
    /// <summary>
    /// 手机短信服务
    /// 参数说明： POST 提交  
    /// 1. itemID  int32  项目编号，由服务提供方统一给予
    /// 2. taskID  int32  任务编号，此次发送任务的编号,保证在项目中唯一
    /// 3. mobile  string 手机号，多个号用英文分号(;)分隔,最多1W个号码
    /// 4. contents string 短信内容，每60个字算一条，超过按多条计算,有中文内容的需做Server.urlencode处理
    /// 5. sendTime datetime  发送时间，值小于等于当前时间，即时发送，大于当前时间，定时发送
    /// 6. _input_charset string 字符编码
    /// 7. sign string 前面各参数哈希值；算法： md5(key1=value1&key2=value2...keyn=valuen+SMSKey(由服务方提供))
    /// 返回值说明：
    /// -1:超过群发限制数
    /// -2:系统出错
    /// -3:手机号不合法
    /// -4:本批次重复发送
    /// -5:签名错误
    /// -6:参数错误
    /// 1:发送成功
    /// </summary>
    public class SMSMessageManager : IMessageManager
    {
        #region Singleton
        private static object lockObj = new object();
        public static SMSMessageManager Instance;
        static SMSMessageManager()
        {
            lock (lockObj)
            {
                if (Instance == null)
                    Instance = new SMSMessageManager();
            }
        }
        private SMSMessageManager()
        { }
        #endregion

        #region Fields
        static string gateway = Lind.DDD.ConfigConstants.ConfigManager.Config.Messaging.SMSGateway;

        static string sign_type = Lind.DDD.ConfigConstants.ConfigManager.Config.Messaging.SMSSignType;
        static string input_charset = Lind.DDD.ConfigConstants.ConfigManager.Config.Messaging.SMSCharset;
        static string key = Lind.DDD.ConfigConstants.ConfigManager.Config.Messaging.SMSKey;
        static int itemID = Lind.DDD.ConfigConstants.ConfigManager.Config.Messaging.SMSItemID;
        #endregion


        #region Private Methods
        /// <summary>
        /// 生成校验字符串
        /// </summary>
        /// <param name="para">参数加密数组</param>
        /// <param name="_input_charset">编码格式</param>
        /// <param name="key">安全校验码</param>
        /// <param name="preValues">返回加密前数据</param>
        /// <returns>字符串URL或加密结果</returns>
        static string CreatSign(
          string[] para,
          string _input_charset,
          string key,
          out string preValues
          )
        {
            //进行排序；
            string[] Sortedstr = BubbleSort(para);
            //构造待md5摘要字符串 ；
            string paraStr = string.Join("&", Sortedstr) + key;
            //生成Md5摘要；
            string sign = GetMD5(paraStr, _input_charset);
            preValues = paraStr;
            return sign;
        }

        /// <summary>
        /// 冒泡排序法
        /// 按照字母序列从a到z的顺序排列
        /// </summary>
        static string[] BubbleSort(string[] r)
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
        static string GetMD5(string s, string _input_charset)
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
        /// 简单的HTTP请求
        /// </summary>
        /// <param name="postData"></param>
        /// <param name="postUrl"></param>
        /// <param name="method"></param>
        /// <param name="encoder"></param>
        /// <returns></returns>
        static string PostMethod(string postData, string postUrl, string method, Encoding encoder)
        {
            try
            {
                if (method == "GET")
                {
                    if (!postUrl.Contains("?"))
                        postUrl = postUrl + "?";

                    HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create(postUrl + postData);
                    req.Method = method;
                    req.Timeout = 3 * 1000;
                    HttpWebResponse myResponse = (HttpWebResponse)req.GetResponse();
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), encoder);
                    return reader.ReadToEnd();
                }
                else if (method == "POST")
                {
                    HttpWebRequest rqt = (HttpWebRequest)HttpWebRequest.Create(postUrl);
                    rqt.Method = method;
                    rqt.Timeout = 3 * 1000;
                    rqt.ContentType = "application/x-www-form-urlencoded";
                    byte[] postdata = encoder.GetBytes(postData);
                    rqt.ContentLength = postdata.Length;
                    Stream writer = rqt.GetRequestStream();
                    writer.Write(postdata, 0, postdata.Length);
                    HttpWebResponse reps = (HttpWebResponse)rqt.GetResponse();

                    HttpWebResponse myResponse = (HttpWebResponse)rqt.GetResponse();
                    StreamReader reader = new StreamReader(myResponse.GetResponseStream(), encoder);
                    return reader.ReadToEnd();
                }
                else
                {
                    return "method参数不对";
                }
            }
            catch (Exception ex)
            {
                return "Error:" + ex.Message;
            }

        }
        #endregion



        public int Send(string recipient, string subject, string body)
        {
            return Send(recipient, subject, body, null, null);
        }

        public int Send(string recipient, string subject, string body, Action errorAction = null, Action successAction = null)
        {
            return Send(new List<string> { recipient }, subject, body, errorAction, successAction);
        }

        public int Send(IEnumerable<string> recipients, string subject, string body, Action errorAction = null, Action successAction = null)
        {
            try
            {
                var taskID = new Random().Next(1, int.MaxValue);
                var channel = 3;
                recipients = recipients.Where(i => new Regex(@"^1[3|4|5|8|7|6][0-9]\d{8}$").IsMatch(i));
                var mobile = string.Join(";", recipients);
                if (!string.IsNullOrWhiteSpace(mobile))
                {
                    var content = string.Concat(subject, ",", body);//subject里需要有个签名，例如【签名名称】
                    var para = new string[]{
                                             "itemID=" + itemID.ToString(),
                                             "taskID=" + taskID.ToString(),
                                             "channel="+channel.ToString(),
                                             "mobile=" + mobile,
                                             "contents=" + content,
                                             "sendTime=" + DateTime.Now.ToString(),
                                             "_input_charset=" + input_charset
                                           };
                    var preValues = string.Empty;
                    var sign = CreatSign(para, input_charset, key, out preValues);
                    var str = "";
                    for (int i = 0; i < para.Length; i++)
                    {
                        if (para[i].Contains("contents"))//对内容部分做 Server.urlencode
                        {
                            str += "contents=" + System.Web.HttpUtility.UrlEncode(content) + "&";
                        }
                        else
                            str += para[i] + "&";
                    }
                    str += "sign=" + sign;
                    var result = PostMethod(str, gateway, "POST", Encoding.GetEncoding("UTF-8"));
                    Lind.DDD.Logger.LoggerFactory.Instance.Logger_Info("手机号" + mobile + ",发送结果：" + result);
                    if (result != "1")
                    {
                        if (errorAction != null)
                        {
                            errorAction();
                        }
                    }
                    else
                    {
                        if (successAction != null)
                            successAction();
                    }
                    int outval;
                    int.TryParse(result, out outval);
                    return outval;
                }
                return 0;
            }
            catch (Exception ex)
            {

                LoggerFactory.Instance.Logger_Info(ex.Message);
                return 0;
            }

        }
    }
}
