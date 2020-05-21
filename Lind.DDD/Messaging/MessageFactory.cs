using Lind.DDD.Messaging.Implements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Messaging
{
    /// <summary>
    /// 消息生产者
    /// 具体消息生产者是单例，如Email,SMS,Rtx等
    /// </summary>
    public sealed class MessageFactory
    {
        /// <summary>
        /// 消息工厂
        /// </summary>
        public static IMessageManager GetService(MessageType messageType)
        {
            switch (messageType)
            {
                case MessageType.Email:
                    return EmailMessageManager.Instance;
                case MessageType.SMS:
                    return SMSMessageManager.Instance;
                case MessageType.RTX:
                    return RTXMessageManager.Instance;
                case MessageType.XMPP:
                    return XMPPMessageManager.Instance;
                default:
                    throw new NotImplementedException("消息生产者未被识别...");
            }
        }

    }
}
