using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Lind.DDD.CacheConfigFile
{
    internal class DataCache
    {
        /// <summary>
        /// 得到cache键所对应的值
        /// </summary>
        /// <param name="CacheKey"></param>
        /// <returns></returns>
        public static object GetCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            return objCache[CacheKey];
        }
        /// <summary>
        /// 将指定值设置到cache键上
        /// </summary>
        /// <param name="CacheKey">键</param>
        /// <param name="objObject">值</param>
        public static void SetCache(string CacheKey, object objObject)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject);
        }
        /// <summary>
        ///  将指定值设置到cache键上
        /// </summary>
        /// <param name="CacheKey">键</param>
        /// <param name="objObject">值</param>
        /// <param name="absoluteExpiration">绝对过期时间</param>
        /// <param name="slidingExpiration">相对过期时间</param>
        public static void SetCache(string CacheKey, object objObject, DateTime absoluteExpiration, TimeSpan slidingExpiration)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Insert(CacheKey, objObject, null, absoluteExpiration, slidingExpiration);
        }

        /// <summary>
        /// 移除指定cache键
        /// </summary>
        /// <param name="CacheKey"></param>
        public static void RemoveCache(string CacheKey)
        {
            System.Web.Caching.Cache objCache = HttpRuntime.Cache;
            objCache.Remove(CacheKey);

        }

    }
}
