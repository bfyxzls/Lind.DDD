using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// Topic队列实体
    /// 用来UI平台的对象返回
    /// </summary>
    public class Topic_Queue_Offset
    {
        /// <summary>
        /// topic名称
        /// </summary>
        public string TopicName { get; set; }
        /// <summary>
        /// 所有的queue的进度
        /// </summary>
        public List<Queue_Offset> Queue_Offset { get; set; }
    }
}
