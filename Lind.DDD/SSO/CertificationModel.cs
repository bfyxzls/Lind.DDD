using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.SSO
{
    /// <summary>
    /// SSO授权模型
    /// </summary>
    public class CertificationModel
    {
        /// <summary>
        /// 用户授权码
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// 产品凭证
        /// </summary>
        public object Certificate { get; set; }
        /// <summary>
        /// 到期时间
        /// </summary>
        public DateTime Expire { get; set; }
    }
}
