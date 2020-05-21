using Lind.DDD.Logger;
using Lind.DDD.Logger.Implements;
using Lind.DDD.Messaging.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.DynamicModules
{
    /// <summary>
    /// function:module design
    /// author:lind
    /// </summary>
    public static class DynamicModuleExtensions
    {
        /// <summary>
        /// logger
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DynamicModule UseLogger(this DynamicModule configuration)
        {
            configuration.RegisterModule<ILogger, NormalLogger>();
            return configuration;
        }

        /// <summary>
        /// message
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DynamicModule UseMessaging(this DynamicModule configuration)
        {
            configuration.RegisterModule<IMessageManager, EmptyMessageManager>();
            return configuration;
        }

        /// <summary>
        /// cache
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DynamicModule UseCache(this DynamicModule configuration)
        {
            configuration.RegisterModule<Lind.DDD.Caching.ICache, Lind.DDD.Caching.RedisCache>();
            return configuration;
        }

        /// <summary>
        /// queue
        /// </summary>
        /// <param name="configuration"></param>
        /// <returns></returns>
        public static DynamicModule UseCacheQueue(this DynamicModule configuration)
        {
            configuration.RegisterModule<Lind.DDD.CachingQueue.IQueue, Lind.DDD.CachingQueue.Implements.MemoryQueue>();
            return configuration;
        }
    }
}
