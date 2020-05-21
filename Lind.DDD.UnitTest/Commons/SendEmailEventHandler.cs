using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lind.DDD.Events;
namespace Lind.DDD.Events.Demo
{
    /// <summary>
    /// 发邮件功能[某个事件的行为]
    /// </summary>
    [Serializable]
    [HandlesAsynchronouslyAttribute]
    public class SendEmailEventHandler :
        IEventHandler<OrderEvent>,
        IEventHandler<UserEvent>,
        IEventHandler<OrderGeneratorEvent>
    {

        #region IEventHandler<OrderEvent> 成员

        public void Handle(OrderEvent evt)
        {
            Console.WriteLine(DateTime.Now + "生成和确认订单{0}时发Email", evt.OrderId);
        }

        #endregion

        #region IEventHandler<UserEvent> 成员

        public void Handle(UserEvent evt)
        {
            Console.WriteLine(DateTime.Now + "建立用户后发Email，用户ＩＤ{0}", evt.UserId);
        }

        #endregion

        #region IEventHandler<OrderGeneratorEvent> 成员

        public void Handle(OrderGeneratorEvent evt)
        {
            Console.WriteLine(DateTime.Now + "订单生成事件，时间{0}", evt.EventTime);
        }

        #endregion
    }
}
