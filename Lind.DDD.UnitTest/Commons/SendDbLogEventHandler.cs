using Lind.DDD.Events;
using Lind.DDD.Events.Demo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest.Commons
{
    [Serializable]
    public class SendDbLogEventHandler : IEventHandler<OrderGeneratorEvent>
    {
        #region IEventHandler<OrderGeneratorEvent> 成员

        public void Handle(OrderGeneratorEvent evt)
        {
            Console.WriteLine("写数据库日志OrderGeneratorEvent" + evt.OrderId);
        }

        #endregion
    }
}
