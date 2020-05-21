using System;
using System.Collections.Generic;

namespace Lind.DDD.Messaging.Implements
{
    /// <summary>
    /// Message Interface
    /// Author:Lind
    /// </summary>
    public interface IMessageManager
    {
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="recipient">接收者</param>
        /// <param name="subject">主题</param>
        /// <param name="body">消息主体</param>
        /// <param name="serverVirtualPath">本参数可以没有，服务端模块级路径，只在xmpp中有意义</param>
        int Send(string recipient, string subject, string body);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="recipient"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <param name="serverVirtualPath"></param>
        /// <param name="errorAction">错误回调</param>
        int Send(string recipient, string subject, string body, Action errorAction = null, Action successAction = null);
        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="recipients">A set of content items to send the message to. Only one message may be sent if the channel manages it.</param>
        /// <param name="type">A custom string specifying what type of message is sent. Used in even handlers to define the message.</param>
        /// <param name="service">The name of the channel to use, e.g. "email"</param>
        /// <param name="properties">A set of specific properties for the channel.</param>
        int Send(IEnumerable<string> recipients, string subject, string body, Action errorAction = null, Action successAction = null);
    }
}
