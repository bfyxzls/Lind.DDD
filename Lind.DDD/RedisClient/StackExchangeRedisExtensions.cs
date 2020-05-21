using RedisUtils;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace StackExchange.Redis
{
    /// <summary>
    /// 对RedisCache的扩展，让它支持复杂类型、
    /// RedisValue 类型可以直接使用字节数组，因此，
    /// 调用 Get 帮助程序方法时，它会将对象序列化为字节流，然后再缓存该对象。
    /// 检索项目时，项目会重新序列化为对象，然后返回给调用程序。
    /// </summary>
    public static class StackExchangeRedisExtensions
    {
        #region Ext Methods
        /// <summary>
        /// 得到键所对应的值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Get<T>(this IDatabase cache, string key)
        {

            return Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<T>(cache.StringGet(key));
        }
        /// <summary>
        /// 得到键所对应的值
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Get(this IDatabase cache, string key)
        {
            return Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<object>(cache.StringGet(key));
        }
        /// <summary>
        /// 设置键对应的值,过期时间后自己删除
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="expireMinutes"></param>
        public static void Set(this IDatabase cache, string key, object value, int expireMinutes)
        {
            string json = Lind.DDD.Utils.SerializeMemoryHelper.SerializeToJson(value);
            cache.StringSet(key, json, TimeSpan.FromMinutes(expireMinutes));
        }
        /// <summary>
        /// 设置键对应的值
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Set(this IDatabase cache, string key, object value)
        {
            cache.Set(key, value, 20);
        }
        /// <summary>
        /// 移除键及值
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        public static void Remove(this IDatabase cache, string key)
        {
            cache.KeyDelete(key);
        }
        /// <summary>
        /// 出队列
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T Pop<T>(this IDatabase cache, string key)
        {
            return Deserialize<T>(cache.ListLeftPop(key));
        }
        /// <summary>
        /// 出队列
        /// 存储为二进制
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object Pop(this IDatabase cache, string key)
        {
            return Deserialize<object>(cache.ListLeftPop(key));
        }
        /// <summary>
        /// 二进制直接出队列
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static byte[] PopByte(this IDatabase cache, string key)
        {
            return cache.ListLeftPop(key);
        }
        /// <summary>
        /// 出队列JSON
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static T PopJson<T>(this IDatabase cache, string key)
        {
            return Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<T>(cache.ListLeftPop(key));
        }
        /// <summary>
        /// 出队列JSON
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object PopJson(this IDatabase cache, string key)
        {
            return Lind.DDD.Utils.SerializeMemoryHelper.DeserializeFromJson<object>(cache.ListLeftPop(key));
        }

        /// <summary>
        /// 入队列
        /// 存储为二进制
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public static void Push(this IDatabase cache, string key, object value)
        {
            cache.ListLeftPush(key, Serialize(value));
        }
        #endregion

        #region Serialize Bin
        /// <summary>
        /// 二进制序列化
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        static byte[] Serialize(object o)
        {
            if (o == null)
            {
                return null;
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream())
            {
                binaryFormatter.Serialize(memoryStream, o);
                byte[] objectDataAsStream = memoryStream.ToArray();
                return objectDataAsStream;
            }
        }
        /// <summary>
        /// 二进制反序列化
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="stream"></param>
        /// <returns></returns>
        static T Deserialize<T>(byte[] stream)
        {
            if (stream == null)
            {
                return default(T);
            }

            BinaryFormatter binaryFormatter = new BinaryFormatter();
            using (MemoryStream memoryStream = new MemoryStream(stream))
            {
                T result = (T)binaryFormatter.Deserialize(memoryStream);
                return result;
            }
        }
        #endregion

        /// <summary>
        /// Redis分页处理集合
        /// </summary>
        /// <param name="redisKey"></param>
        /// <param name="pageSize"></param>
        /// <param name="action"></param>
        public static void PagerAction(string redisKey, int pageSize, Action<StackExchange.Redis.RedisValue[]> action)
        {
            int _pagesize = pageSize <= 1 ? pageSize : pageSize - 1;
            for (int i = 0; i < RedisManager.Instance.GetDatabase(1).ListLength(redisKey); i += _pagesize)
            {
                if (i < RedisManager.Instance.GetDatabase(1).ListLength(redisKey) - 1)
                {
                    var val = RedisManager.Instance.GetDatabase(1).ListRange(redisKey, start: i, stop: i + _pagesize);
                    action(val);
                }

            }
        }

    }

}
