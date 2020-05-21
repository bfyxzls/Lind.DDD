using Lind.DDD.Utils.Encryptor;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Authorization.Api
{
    /// <summary>
    /// api校验相关
    /// </summary>
    public class ApiValidateHelper
    {
        /// <summary>
        /// 密文键
        /// </summary>
        public const string CipherText = "ciphertext";
        /// <summary>
        /// 生成秘文(appkey和passkey从配置文件节点获取)
        /// 并返回[在url上加这个键ciphertext]
        /// </summary>
        /// <param name="coll"></param>
        /// <returns></returns>
        public static NameValueCollection GenerateCipherText(NameValueCollection coll, bool isClearCode = false)
        {
            string appKey = System.Configuration.ConfigurationManager.AppSettings["Api_AppKey"];
            string passKey = System.Configuration.ConfigurationManager.AppSettings["Api_PassKey"];
            return GenerateCipherText(coll, appKey, passKey, isClearCode);
        }
        /// <summary>
        /// 生成秘文，并返回[在url上加这个键ciphertext]
        /// </summary>
        /// <param name="coll">已有的集合</param>
        /// <param name="appKey">当前项目的appKey</param>
        /// <param name="passKey">appkey对应的passKey</param>
        /// <returns></returns>
        public static NameValueCollection GenerateCipherText(NameValueCollection coll, string appKey, string passKey, bool isClearCode = false)
        {
            if (coll == null)
                coll = new NameValueCollection();
            var timeStamp = (DateTime.UtcNow - new DateTime(1970,1,1)).TotalSeconds.ToString();//统一的UTC时间戳
            if (string.IsNullOrWhiteSpace(coll.Get("AppKey")))
                coll.Add("AppKey", appKey);
            if (string.IsNullOrWhiteSpace(coll.Get("timeStamp")))
                coll.Add("timeStamp", timeStamp);
            var paramStr = new StringBuilder();
            var keys = new List<string>();


            #region 验证算法
            foreach (string param in coll.Keys)
            {
                if (!string.IsNullOrEmpty(param))
                {
                    keys.Add(param.ToLower());
                }

            }
            keys.Sort();
            foreach (string p in keys)
            {
                if (!string.IsNullOrEmpty(coll[p]))
                {
                    paramStr.Append(coll[p]);
                }
            }

            paramStr.Append(passKey);
            #endregion

            if (isClearCode)//post,put,不需要保留明文，只需要保存appkey和passkey
            {
                coll = new NameValueCollection();//清空明文列表
                coll.Add("AppKey", appKey);
                coll.Add("timeStamp", timeStamp);
            }
            if (string.IsNullOrWhiteSpace(coll.Get(CipherText)))//避免重复添加
                coll.Add(CipherText, Utility.EncryptString(paramStr.ToString(), Utility.EncryptorType.MD5));
            return coll;
        }
        /// <summary>
        /// 生成密文(appkey和passkey从配置文件节点获取)
        /// 把对象转成k/v串并加密返回密文（密文一般会添加到URL上参数名为CipherText）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static NameValueCollection GenerateCipherText<T>(List<T> list, bool isClearCode = false)
        {
            string appKey = System.Configuration.ConfigurationManager.AppSettings["Api_AppKey"];
            string passKey = System.Configuration.ConfigurationManager.AppSettings["Api_PassKey"];
            return GenerateCipherText(list, appKey, passKey, isClearCode);
        }
        /// <summary>
        /// 把对象转成k/v串并加密返回密文（密文一般会添加到URL上参数名为CipherText）
        /// </summary>
        /// <param name="obj">对象</param>
        /// <param name="appKey">项目的key</param>
        /// <param name="passKey">为项目颁发的密钥</param>
        /// <returns></returns>
        public static NameValueCollection GenerateCipherText<T>(List<T> list, string appKey, string passKey, bool isClearCode = false)
        {
            var nv = new NameValueCollection(); ;
            var prefix = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {
                prefix.Add("Root_" + i.ToString());
                ReGenerate(list[i], i, prefix, nv);
                prefix.Clear();
            }

            return GenerateCipherText(nv, appKey, passKey, isClearCode);
        }
        /// 生成密文(appkey和passkey从配置文件节点获取)
        /// 把对象转成k/v串并加密返回密文（密文一般会添加到URL上参数名为CipherText）
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static NameValueCollection GenerateCipherText<T>(T entity, bool isClearCode = false)
        {
            return GenerateCipherText<T>(new List<T> { entity }, isClearCode);
        }
        /// <summary>
        /// 把对象转成k/v串并加密返回密文（密文一般会添加到URL上参数名为CipherText）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="appKey"></param>
        /// <param name="passKey"></param>
        /// <returns></returns>
        public static NameValueCollection GenerateCipherText<T>(T entity, string appKey, string passKey, bool isClearCode = false)
        {
            return GenerateCipherText(new List<T> { entity }, appKey, passKey, isClearCode);
        }
        static void ReGenerate(object obj, int i, List<string> prefix, NameValueCollection nv)
        {
            if (obj != null)
            {
                prefix.Add(obj.GetType().Name);

                foreach (var item in obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (item.PropertyType.IsClass && item.PropertyType != typeof(string))
                    {
                        var sub = item.GetValue(obj);
                        ReGenerate(sub, i, prefix, nv);
                    }
                    else
                    {
                        if (item.GetValue(obj) != null)
                            nv.Add(string.Join("_", prefix) + "_" + item.Name, item.GetValue(obj).ToString());
                    }
                }
            }
        }
    }
}
