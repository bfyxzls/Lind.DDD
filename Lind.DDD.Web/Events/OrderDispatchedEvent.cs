using Lind.DDD.Events;
using System;

namespace Lind.DDD.Web.Events
{
    /// <summary>
    /// 表示当针对某销售订单进行发货时所产生的领域事件。
    /// </summary>
    [Serializable]
    public class OrderDispatchedEvent : EventData
    {
        public int UserId { get; set; }
        public int OrderId { get; set; }

    }
}
