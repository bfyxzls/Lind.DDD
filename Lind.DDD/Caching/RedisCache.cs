using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Caching
{
    /// <summary>
    /// 使用redis
    /// </summary>
    public class RedisCache : ICache
    {
        readonly static string _conn = ConfigConstants.ConfigManager.Config.Redis.Host;
        readonly static int _expireMinutes = ConfigConstants.ConfigManager.Config.Caching.ExpireMinutes;
        readonly static StackExchange.Redis.IDatabase cache = StackExchange.Redis.ConnectionMultiplexer.Connect(_conn).GetDatabase();//redis0:6380,redis1:6380,allowAdmin=true

        #region ICache 成员

        public void Put(string key, object obj)
        {
            cache.Set(key, obj, _expireMinutes);
        }

        public void Put(string key, object obj, int expireMinutes)
        {
            cache.Set(key, obj, expireMinutes);
        }

        public object Get(string key)
        {
            return cache.Get(key);
        }

        public void Delete(string key)
        {
            cache.Remove(key);
        }

        #endregion
    }


}
