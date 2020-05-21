using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Security.Permissions;
using System.Text;
using Lind.DDD.Events;
namespace Lind.DDD.Events.Demo
{
    /// <summary>
    /// 添加订单的事件[事件实体]
    /// </summary>
    [Serializable]
    public class OrderGeneratorEvent : EventData
    {
        public DateTime ConfirmedDate { get; set; }
        public string UserEmailAddress { get; set; }
        public Guid OrderId { get; set; }
        public string Title { get; set; }
        //Public IUnitOfWork UnitOfWork { get; set; } //数据上下文
     }

    /// <summary>
    /// 表示订单确认的领域事件。
    /// </summary>
    [Serializable]
    public class OrderConfirmedEvent : EventData
    {
        #region Public Properties
        /// <summary>
        /// 获取或设置订单确认的日期。
        /// </summary>
        public DateTime ConfirmedDate { get; set; }
        public string UserEmailAddress { get; set; }
        public Guid OrderId { get; set; }
        public string Title { get; set; }

        #endregion
    }
}
