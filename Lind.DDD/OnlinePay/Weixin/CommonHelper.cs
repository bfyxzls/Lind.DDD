using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.OnlinePay.Weixin
{
    public class CommonHelper
    {
        public static string CreateSign(Dictionary<string, string> paramDict)
        {
            return CreateSign(paramDict, null);
        }

        public static string CreateSign(Dictionary<string, string> paramDict, Dictionary<string, int> intParamDict)
        {
            if (paramDict == null && intParamDict == null)
                throw new Exception("No params to sign!");
            string string1 = GetString1(paramDict, intParamDict);
            return GetSign(string1);
        }

        public static string CreateXmlForReturn(Dictionary<string, string> dict)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("<xml>");
            foreach (var item in dict)
            {
                sb.AppendLine(string.Format("<{0}>{1}</{0}>", item.Key, item.Value));
            }
            sb.AppendLine("</xml>");
            return sb.ToString();
        }

        /// <summary>
        /// 对所有传入参数按照字段名的 ASCII 码从小到大排序（字典序）后，使用 URL 键值对的
        ///  格式（即 key1=value1&key2=value2…）拼接成字符串 string1
        /// </summary>
        /// <returns></returns>
        private static string GetString1(Dictionary<string, string> paramDict, Dictionary<string, int> intParamDict)
        {
            List<string> list = new List<string>();
            if (paramDict == null)
                paramDict = new Dictionary<string, string>();
            if (intParamDict == null)
                intParamDict = new Dictionary<string, int>();
            foreach (var item in paramDict)
            {
                if (item.Key != "sign"&&!string.IsNullOrEmpty(item.Value))
                    list.Add(item.Key);
            }
            foreach (var item in intParamDict)
            {
                list.Add(item.Key);
            }
            string[] arr = list.ToArray();
            Array.Sort(arr);

            StringBuilder sb = new StringBuilder();
            foreach (var item in arr)
            {
                if (paramDict.ContainsKey(item))
                    sb.Append(string.Format("{0}={1}&", item, paramDict[item]));
                else
                    sb.Append(string.Format("{0}={1}&", item, intParamDict[item]));
            }
            string result = sb.ToString();
            if (!string.IsNullOrEmpty(result))
                result = result.TrimEnd('&');
            return result;
        }

        private static string GetSign(string string1)
        {
            string key=PaymentConfig.Instance.Key;
            string stringSignTemp = string.Format("{0}&key={1}", string1, key);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] encrypt = Encoding.UTF8.GetBytes(stringSignTemp);
            byte[] resultEncrypt = md5.ComputeHash(encrypt);
            StringBuilder sb = new StringBuilder();
            foreach (var item in resultEncrypt)
            {
                sb.Append(item.ToString("X2"));
            }
            return sb.ToString().ToUpper();
        }


    }
}
