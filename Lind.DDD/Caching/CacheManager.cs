using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Caching
{
    /// <summary>
    /// 缓存管理者
    /// </summary>
    public class CacheManager : ICache
    {
        #region Constructors
        /// <summary>
        /// 类构造方法，对外不支持创建它的实例对象
        /// </summary>
        static CacheManager() { }

        /// <summary>
        /// 私有构造方法，对外不公开
        /// </summary>
        private CacheManager()
        {
            switch (_cacheProvider.ToLower())
            {
                case "runtimecache":
                    _iCache = new RuntimeCache();
                    break;
                case "rediscache":
                    _iCache = new RedisCache();
                    break;
                default:
                    throw new ArgumentException("缓存提供者只支持RunTimeCache和RedisCache");
            }
        }
        #endregion

        /// <summary>
        /// 缓存管理者单例
        /// </summary>
        public static CacheManager Instance
        {
            get
            {
                lock (lockObj)
                {
                    if (_instance == null)
                        _instance = new CacheManager();
                    return _instance;
                }

            }
        }

        #region Fields
        private static object lockObj = new object();
        private static CacheManager _instance;
        private ICache _iCache;
        private static string _cacheProvider = ConfigConstants.ConfigManager.Config.Caching.Provider ?? "RuntimeCache";
        #endregion

        #region ICache 成员

        public void Put(string key, object obj)
        {
            _iCache.Put(key, obj);
        }

        public void Put(string key, object obj, int expireMinutes)
        {
            _iCache.Put(key, obj, expireMinutes);
        }

        public object Get(string key)
        {
            return _iCache.Get(key);
        }

        public void Delete(string key)
        {
            _iCache.Delete(key);
        }

        #endregion
    }
}
