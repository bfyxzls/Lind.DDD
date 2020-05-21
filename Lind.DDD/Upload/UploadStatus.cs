using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Upload
{
    /// <summary>
    /// 上传状态
    /// </summary>
    public enum UploadStatus
    {
        /// <summary>
        /// 成功
        /// </summary>
        Success,
        /// <summary>
        /// 中断
        /// </summary>
        Interrupted,
        /// <summary>
        /// 失败
        /// </summary>
        Faild
    }
}
