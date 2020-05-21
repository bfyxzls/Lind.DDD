using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Authorization.Api
{
    /// <summary>
    /// 配置文件对象
    /// </summary>
    public class ApiValidateModelConfig : Lind.DDD.CacheConfigFile.IConfiger
    {
        public ApiValidateModelList ApiValidateModelList { get; set; }
    }
 
    /// <summary>
    /// 配置列表
    /// </summary>
    [Serializable]
    public class ApiValidateModelList : List<ApiValidateModel> { }

    /// <summary>
    /// 服务端－客户端数据校验模型
    /// </summary>
    [Serializable]
    public class ApiValidateModel
    {
        /// <summary>
        /// 项目键，用于网络传输
        /// </summary>
        public string AppKey { get; set; }
        /// <summary>
        /// 项目名称
        /// </summary>
        public string AppName { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string PassKey { get; set; }
        /// <summary>
        /// 过期时间
        /// </summary>
        public DateTime ExpireDate { get; set; }
        /// <summary>
        /// 一个连接的有效分钟
        /// </summary>
        public int ValidateMinutes { get; set; }
    }
}
