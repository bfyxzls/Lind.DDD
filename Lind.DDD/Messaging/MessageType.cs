using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.Messaging
{
    /// <summary>
    /// 消息类型:0 email,1 sms,2 rtx
    /// </summary>
    [Flags]
    public enum MessageType
    {
        /// <summary>
        /// 电子邮件
        /// </summary>
        Email = 1,
        /// <summary>
        /// 短信息
        /// </summary>
        SMS = 2,
        /// <summary>
        /// RTX实时通讯
        /// </summary>
        RTX = 3,
        /// <summary>
        /// XMPP消息推送
        /// </summary>
        XMPP = 4,

    }
}
