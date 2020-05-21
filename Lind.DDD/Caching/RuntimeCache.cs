using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Lind.DDD.Caching
{
    /// <summary>
    /// 运行时缓存，基于服务端内存存储
    /// </summary>
    public class RuntimeCache : ICache
    {
        readonly static System.Web.Caching.Cache httpRuntimeCache = System.Web.HttpRuntime.Cache;
        readonly static int _expireMinutes = ConfigConstants.ConfigManager.Config.Caching.ExpireMinutes;

        #region ICache 成员

        public void Put(string key, object obj)
        {
            httpRuntimeCache.Insert(key, obj);
        }

        public void Put(string key, object obj, int expireMinutes)
        {
            httpRuntimeCache.Insert(key, obj, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromMinutes(1));
        }

        public object Get(string key)
        {
            return httpRuntimeCache.Get(key);
        }

        public void Delete(string key)
        {
            httpRuntimeCache.Remove(key);
        }

        #endregion
    }
}
