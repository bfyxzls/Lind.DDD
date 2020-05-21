using Lind.DDD.RedisUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using Lind.DDD.Caching;
using RedisUtils;

namespace Lind.DDD.CachingDataSet
{
    /// <summary>
    ///使用redis方式进行缓存持久化
    /// </summary>
    internal class RedisCacheProvider : ICacheProvider
    {
        IDatabase db = RedisManager.Instance.GetDatabase();
        static byte[] Serialize(object data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream();
            formatter.Serialize(rems, data);
            return rems.GetBuffer();
        }
        static object Deserialize(byte[] data)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream rems = new MemoryStream(data);
            data = null;
            return formatter.Deserialize(rems);
        }
        public void Add(string key, string valKey, object value)
        {
            byte[] byteValue = Serialize(value);
            var tbl = db.Get<Dictionary<string, byte[]>>(key);

            if (tbl == null)
            {
                tbl = new Dictionary<string, byte[]>();
                tbl.Add(valKey, byteValue);
            }
            else
            {
                tbl[valKey] = byteValue;
            }
        }

        public void Put(string key, string valKey, object value)
        {
            Add(key, valKey, value);
        }

        public object Get(string key, string valKey)
        {
            var tbl = db.Get<Dictionary<string, byte[]>>(key);
            if (tbl != null)
            {
                if (tbl.ContainsKey(valKey))
                    return Deserialize(tbl[valKey]);
                return null;
            }
            return null;
        }

        public void Remove(string key)
        {
            db.KeyDelete(key);
        }

        public bool Exists(string key)
        {
            return db.KeyExists(key);
        }

        public bool Exists(string key, string valKey)
        {
            var tbl = db.Get<Dictionary<string, byte[]>>(key);
            return tbl != null && tbl.ContainsKey(valKey);
        }
    }
}
