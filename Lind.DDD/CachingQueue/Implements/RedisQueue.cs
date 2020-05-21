using Lind.DDD.RedisUtils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using RedisUtils;

namespace Lind.DDD.CachingQueue.Implements
{
    public class RedisQueue : IQueue
    {
        private readonly IDatabase _cacheManager = RedisManager.Instance.GetDatabase();
        private const string CACHE_QUEUE_KEY = "LindCacheQueue";
        #region IQueue 成员

        public void Push(string key, byte[] obj)
        {
            _cacheManager.ListLeftPush(CACHE_QUEUE_KEY + "_" + key, obj);
        }

        public byte[] Pop(string key)
        {
            return _cacheManager.ListLeftPop(CACHE_QUEUE_KEY + "_" + key);
        }

        public int Count
        {
            get
            {
                return (int)_cacheManager.ListLength(CACHE_QUEUE_KEY);
            }
        }

        #endregion
    }
}
