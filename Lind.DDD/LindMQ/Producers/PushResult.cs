using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 推送消息的结果
    /// </summary>
    public class PushResult
    {
        /// <summary>
        /// 发送的状态
        /// </summary>
        public PushStatus PushStatus { get; set; }
        /// <summary>
        /// 失败时的消息
        /// </summary>
        public string ErrorMessage { get; set; }
    }
}
