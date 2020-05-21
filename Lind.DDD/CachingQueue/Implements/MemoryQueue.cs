using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CachingQueue.Implements
{
    /// <summary>
    /// 默认的队列
    /// </summary>
    public class MemoryQueue : IQueue
    {
        private static readonly Queue<byte[]> _queue = new Queue<byte[]>();
        #region IQueue 成员

        public void Push(string key, byte[] obj)
        {
            _queue.Enqueue(obj);
        }

        public byte[] Pop(string key)
        {
            if (this.Count > 0)
                return _queue.Dequeue();
            return null;
        }

        public int Count
        {
            get
            {
                return _queue.Count;
            }
        }

        #endregion
    }
}
