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
    [XmlRoot("SentResult")]
    public class SentResult
    {
        /// <summary>
        /// 数据库及表名称组合
        /// </summary>
        public string Db_TableName { get; set; }
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


    }

}
