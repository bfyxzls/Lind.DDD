using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.RabbitMQ
{
    /// <summary>
    /// 消息接收事件源对象
    /// </summary>
    public class MessageReceivedEventArgs : EventArgs
    {
        public MessageReceivedEventArgs(object message)
        {
            this.Message = message;
        }
        public MessageReceivedEventArgs()
        {

        }
        /// <summary>
        /// 消息体
        /// </summary>
        public object Message { get; set; }
    }
}
