using System;
using System.Web;

namespace Lind.DDD.Utils
{

    /// <summary>
    /// 缓存相关的操作类
    /// 占占
    /// </summary>
    public class DataCache
    {
        /// <summary>
        /// 获取当前应用程序指定CacheKey的Cache值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];

        }
        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// 调用：DataCache.SetCache("name", "zzl");
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }

        /// <summary>
        /// 设置当前应用程序指定CacheKey的Cache值
        /// 调用：DataCache.SetCache("name", "zzl", DateTime.Now.AddMinutes(1), TimeSpan.Zero);
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <param name="objObject"></param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 移除指定Cache
        /// </summary>
        /// <param name="CacheKey"></param>
        public static void RemoveCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Remove(CacheKey);

        }

        #region Events
        /// <summary>
        /// 缓存删除事件
        /// </summary>
        public static event CacheEventHandler CacheDeleted;
        #endregion

        #region Methods
        /// <summary>
        /// 触发缓存删除事件
        /// </summary>
        public static void OnCacheDeleted(string key)
        {
            if ((CacheDeleted != null))
            {
                CacheDeleted(null, new CacheEventArgs(key));
            }

            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Remove(key);

        }
        #endregion
    }

    #region Delegates
    /// <summary>
    /// 缓存委托
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void CacheEventHandler(object sender, CacheEventArgs e);
    #endregion

    /// <summary>
    /// 缓存事件源
    /// </summary>
    public class CacheEventArgs
    {
        public CacheEventArgs()
        {

        }
        public CacheEventArgs(string cacheKey)
        {
            this.CacheKey = cacheKey;
        }
        public string CacheKey { get; set; }
    }
}
