using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lind.DDD.Events;
namespace Lind.DDD.Events.Demo
{
    /// <summary>
    /// 表示订单确认的领域事件。
    /// </summary>
    [Serializable]
    public class OrderEvent : EventData
    {
        #region Public Properties
        /// <summary>
        /// 获取或设置订单确认的日期。
        /// </summary>
        public DateTime ConfirmedDate { get; set; }
        public DateTime GeneratorDate { get; set; }
        public string UserEmailAddress { get; set; }
        public Guid OrderId { get; set; }
        public string Title { get; set; }

        #endregion
    }
}
