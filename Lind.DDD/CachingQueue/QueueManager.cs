using Lind.DDD.CachingQueue.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CachingQueue
{
    /// <summary>
    /// 队列管理器
    /// appsetting去配置队列方面，它可以是Default,File和Redis
    /// </summary>
    public sealed class QueueManager : IQueue
    {
        private static object _lockObj = new object();

        private IQueue _iQueue = new MemoryQueue();//这里可以走策略模式

        public QueueManager()
        {
            string queueType = ConfigConstants.ConfigManager.Config.Queue.Type ?? "Memory";
            switch (queueType)
            {
                case "Memory":
                    _iQueue = new MemoryQueue();
                    break;
                case "File":
                    _iQueue = new FileQueue();
                    break;
                case "Redis":
                    _iQueue = new RedisQueue();
                    break;
                default:
                    throw new ArgumentException("队列存储方式不正确目前只支持Memory,File和Redis");
            }
        }
        /// <summary>
        /// 队列实例（单例）
        /// </summary>
        public static QueueManager Instance
        {
            get
            {
                lock (_lockObj)
                {
                    return new QueueManager();
                }
            }
        }


        #region IQueue 成员

        public void Push(string key, byte[] obj)
        {
            _iQueue.Push(key, obj);
        }

        public byte[] Pop(string key)
        {
            return _iQueue.Pop(key);
        }

        public int Count
        {
            get { return _iQueue.Count; }
        }

        #endregion
    }
}
