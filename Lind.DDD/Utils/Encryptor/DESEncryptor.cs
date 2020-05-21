using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace Lind.DDD.Utils.Encryptor
{
    /// <summary>
    /// DES加密类
    /// </summary>
    internal class DESEncryptor
    {
        private string sKey;
        public DESEncryptor()
        {
            int[] numArray = new int[] { 0x7b, 0xea, 0xc3, 0xa5, 0xc9, 240, 0x8f, 0xc6 };
            foreach (int num in numArray)
            {
                this.sKey = this.sKey + ((char)num).ToString();
            }
        }


        /// <summary>
        /// DES解密
        /// </summary>
        /// <param name="InputConnectionString"></param>
        /// <returns></returns>
        public string DecryptString(string InputConnectionString)
        {
            if (InputConnectionString.Equals(string.Empty))
            {
                return InputConnectionString;
            }
            return _Decrypt(InputConnectionString);
        }

        /// <summary>
        /// DES加密
        /// </summary>
        /// <param name="encryptedString"></param>
        /// <returns></returns>
        public string EncryptString(string encryptedString)
        {
            return _Encrypt(encryptedString);
        }


        private string _Decrypt(string pToDecrypt)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] buffer = new byte[pToDecrypt.Length / 2];
            for (int i = 0; i < (pToDecrypt.Length / 2); i++)
            {
                int num2 = Convert.ToInt32(pToDecrypt.Substring(i * 2, 2), 0x10);
                buffer[i] = (byte)num2;
            }
            provider.Key = Encoding.ASCII.GetBytes(this.sKey);
            provider.IV = Encoding.ASCII.GetBytes(this.sKey);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateDecryptor(), CryptoStreamMode.Write);
            stream2.Write(buffer, 0, buffer.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            return Encoding.Default.GetString(stream.ToArray());
        }

        private string _Encrypt(string pToEncrypt)
        {
            DESCryptoServiceProvider provider = new DESCryptoServiceProvider();
            byte[] bytes = Encoding.Default.GetBytes(pToEncrypt);
            provider.Key = Encoding.ASCII.GetBytes(this.sKey);
            provider.IV = Encoding.ASCII.GetBytes(this.sKey);
            MemoryStream stream = new MemoryStream();
            CryptoStream stream2 = new CryptoStream(stream, provider.CreateEncryptor(), CryptoStreamMode.Write);
            stream2.Write(bytes, 0, bytes.Length);
            stream2.FlushFinalBlock();
            StringBuilder builder = new StringBuilder();
            foreach (byte num in stream.ToArray())
            {
                builder.AppendFormat("{0:X2}", num);
            }
            builder.ToString();
            return builder.ToString();
        }

    }
}
