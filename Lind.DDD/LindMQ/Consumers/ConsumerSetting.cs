using Lind.DDD.FastSocket.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 消费模型
    /// </summary>
    public class ConsumerSetting
    {
        /// <summary>
        /// 中间件别名
        /// </summary>
        public string BrokenName { get; set; }
        /// <summary>
        /// 中间件地址
        /// </summary>
        public IPEndPoint BrokenAddress { get; set; }
        /// <summary>
        /// 消费topic和对应的处理程序，一般来说，每个topic都有自己的处理程序
        /// </summary>
        public Dictionary<string, Action<MessageBody>> Callback { get; set; }
        /// <summary>
        /// 客户端连接
        /// </summary>
        public AsyncBinarySocketClient Client { get; set; }

    }

}
