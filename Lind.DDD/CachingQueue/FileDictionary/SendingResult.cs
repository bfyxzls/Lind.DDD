using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Lind.DDD.CachingQueue.FileDictionary
{
    /// <summary>
    /// 数据同步结果对象
    /// </summary>
    [XmlRoot("SendingResult")]
    public class SendingResult
    {
        /// <summary>
        /// 批次号，本次与服务端交互时，唯王标识
        /// </summary>
        public string BatchNumber { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 数据表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 批次开始主键号
        /// </summary>
        public long StartId { get; set; }
        /// <summary>
        /// 批次结束主键号
        /// </summary>
        public long EndId { get; set; }
        /// <summary>
        /// 事件发生的时间
        /// </summary>
        public DateTime OccurDate { get; set; }
    }
}
