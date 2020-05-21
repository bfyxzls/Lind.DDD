using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 队列消费进度
    /// </summary>
    public class Queue_Offset
    {
        /// <summary>
        /// 队列完整名称，包括了topic名称
        /// </summary>
        public string QueueName { get; set; }
        /// <summary>
        /// 消息体
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 消费进度
        /// </summary>
        public int Score { get; set; }
    }
}
