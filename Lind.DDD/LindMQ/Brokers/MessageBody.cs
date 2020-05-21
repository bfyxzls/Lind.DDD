using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 消息协议
    /// </summary>
    [Serializable]
    public class MessageBody
    {
        public MessageBody()
        {
            CreateTime = DateTime.Now;
        }
        /// <summary>
        /// 消息所属Topic，每种Topic有一种类型的Body
        /// </summary>
        public string Topic { get; set; }
        /// <summary>
        /// 消息内容，Redis里存储为Json
        /// </summary>
        public string Body { get; set; }
        /// <summary>
        /// 消息所属的队列ID
        /// </summary>
        public int QueueId { get; set; }
        /// <summary>
        /// 消息在所属队列的序号
        /// </summary>
        public long QueueOffset { get; set; }
        /// <summary>
        /// 消息的存储时间
        /// </summary>
        public DateTime CreateTime { get; private set; }
        /// <summary>
        /// 将消息对象序列化成字符
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Utils.SerializeMemoryHelper.SerializeToJson<MessageBody>(this);
        }
    }




}
