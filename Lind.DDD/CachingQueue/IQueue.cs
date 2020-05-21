using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CachingQueue
{
    /// <summary>
    /// 队列标准
    /// </summary>
    public interface IQueue
    {
        /// <summary>
        /// 添加到队列(FIFO)
        /// </summary>
        /// <param name="key">键名</param>
        /// <param name="obj">值</param>
        void Push(string key, byte[] obj);
        /// <summary>
        /// 从队列中取出(FIFO)
        /// </summary>
        /// <param name="key">键名</param>
        /// <returns>值</returns>
        byte[] Pop(string key);
        /// <summary>
        /// 得到队列的项目总数
        /// </summary>
        /// <returns></returns>
        int Count { get; }
    }
}
