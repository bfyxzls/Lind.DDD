using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

namespace RedisUtils
{
    /// <summary>
    /// 序列化与反序列化到内存
    /// </summary>
    public class SerializeMemoryHelper
    {

        public static long GetByteSize(object o)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                bFormatter.Serialize(stream, o);
                return stream.Length;
            }
        }

   

        #region JSON
        /// <summary>
        /// 字符串反序列化
        /// </summary>
        /// <param name="strBase64"></param>
        /// <returns></returns>
        public static T DeserializeFromJson<T>(string jsonStr)
        {
            return JsonDeserialize<T>(jsonStr);
        }
        /// <summary>
        /// 字符串序列化
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static string SerializeToJson<T>(T obj)
        {
            return JsonSerializer<T>(obj);
        }
        #endregion

        #region JSON Method

        /// <summary>
        /// JSON序列化
        /// </summary>
        private static string JsonSerializer<T>(T t)
        {
            try
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                return Newtonsoft.Json.JsonConvert.SerializeObject(t);
            }
            catch (Exception)
            {
                JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };//解决大小的限制
                return js.Serialize(t);
            }

        }
        /// <summary>
        /// JSON反序列化
        /// </summary>
        private static T JsonDeserialize<T>(string jsonString)
        {
            try
            {
                var settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.All };
                return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(jsonString);
            }
            catch (MissingMethodException)
            {
                JavaScriptSerializer js = new JavaScriptSerializer() { MaxJsonLength = int.MaxValue };
                return js.Deserialize<T>(jsonString); //这版在反序列化Tuple时会有问题，因为它没有空构造方法
            }
        }

        /// <summary>
        /// 将Json序列化的时间由/Date(1294499956278+0800)转为字符串
        /// </summary>
        private static string ConvertJsonDateToDateString(Match m)
        {
            string result = string.Empty;
            DateTime dt = new DateTime(1970, 1, 1);
            dt = dt.AddMilliseconds(long.Parse(m.Groups[1].Value));
            dt = dt.ToLocalTime();
            result = dt.ToString("yyyy-MM-dd HH:mm:ss");
            return result;
        }
        /// <summary>
        /// 将时间字符串转为Json时间
        /// </summary>
        private static string ConvertDateStringToJsonDate(Match m)
        {
            string result = string.Empty;
            DateTime dt = DateTime.Parse(m.Groups[0].Value);
            dt = dt.ToUniversalTime();
            TimeSpan ts = dt - DateTime.Parse("1970-01-01");
            result = string.Format("\\/Date({0}+0800)\\/", ts.TotalMilliseconds);
            return result;
        }
        #endregion
    }

}