using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;

namespace Lind.DDD.Messaging
{
    /// <summary>
    /// 消息实体
    /// </summary>
    public class MessageContext
    {
        /// <summary>
        /// 消息类型
        /// </summary>
        public MessageType Type { get; set; }
        /// <summary>
        /// 消息头
        /// </summary>
        public string Subject { get; set; }
        /// <summary>
        /// 消息正文
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 接受方地址列表
        /// </summary>
        public IEnumerable<string> Addresses { get; set; }
        /// <summary>
        /// 是否处于准备发送状态
        /// </summary>
        public bool MessagePrepared { get; set; }

        public MessageContext()
        {
            Addresses = Enumerable.Empty<string>();//这时Addresses!=null,使用Addresses.ToList().ForEach(i => Console.WriteLine(i));不会引发异常
        }
    }
}
