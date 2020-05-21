using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 客户端对于Topic队列的消费进度
    /// </summary>
    public class Client_TopicQueue_Offset
    {
        /// <summary>
        /// 客户端消费者标识，可能是ＩＰ地址
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Topic名称
        /// </summary>
        public string TopicName { get; set; }
        /// <summary>
        /// 队列编号
        /// </summary>
        public int QueueId { get; set; }
        /// <summary>
        /// 队列消费的偏移量
        /// </summary>
        public int Offset { get; set; }
    }
}
