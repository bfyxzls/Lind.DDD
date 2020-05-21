using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 生产者相关配置
    /// </summary>
    public class ProducerSetting
    {
        /// <summary>
        /// Broker服务器名称
        /// </summary>
        public string BrokerName { get; set; }
        /// <summary>
        /// Broker服务器地址
        /// </summary>
        public string BrokerAddress { get; set; }
        /// <summary>
        /// Broker服务器端口
        /// </summary>
        public int BrokerPort { get; set; }
        /// <summary>
        /// 超时时间
        /// </summary>
        public int Timeout { get; set; }
    }
}
