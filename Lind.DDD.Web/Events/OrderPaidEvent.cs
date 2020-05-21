using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Web.Events
{
    /// <summary>
    /// 订单付款事件源
    /// </summary>
    public class OrderPaidEvent : Lind.DDD.Events.EventData
    {
        public int OrderId { get; set; }
        public int UserId { get; set; }
    }
}
