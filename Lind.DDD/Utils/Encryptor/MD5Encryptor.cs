using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

namespace Lind.DDD.Utils.Encryptor
{
    internal class MD5Encryptor
    {
        /// <summary>
        /// MD5函数
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            if (string.IsNullOrEmpty(str))
                return "";
            byte[] b = Encoding.Default.GetBytes(str);
            b = new MD5CryptoServiceProvider().ComputeHash(b);
            StringBuilder ret = new StringBuilder();
            for (int i = 0; i < b.Length; i++)
                ret.Append(b[i].ToString("x").PadLeft(2, '0'));
            return ret.ToString();
        }

        /// <summary>
        /// 代有加密码长度的ＭＤ５
        /// </summary>
        /// <param name="str"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string MD5(string str, int code)
        {
            if (code == 16)
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5").ToLower().Substring(8, 16);
            }
            if (code == 32)
            {
                return System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "MD5");
            }
            return "00000000000000000000000000000000";
        }
    }
}
