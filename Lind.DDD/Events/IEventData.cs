using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.Events
{
    /// <summary>
    /// 领域事件实体基类[实体接口]
    /// </summary>
    public interface IEventData
    {
        /// <summary>
        /// 领域事件实体的聚合根，生命周期在会话结束后消失
        /// </summary>
        Guid AggregateRoot { get; }
        /// <summary>
        /// 事件发生时间
        /// </summary>
        DateTime EventTime { get; }
        /// <summary>
        /// 事件模型
        /// </summary>
        object EventModel { get; set; }
    }
}
