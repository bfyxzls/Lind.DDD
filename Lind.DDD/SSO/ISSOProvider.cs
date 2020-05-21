using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.SSO
{
    /// <summary>
    /// SSO持久化的提供者
    /// </summary>
    public interface ISSOProvider
    {
        /// <summary>
        /// 获取缓存中的DataTable
        /// </summary>
        /// <returns></returns>
        List<CertificationModel> GetCacheTable();
        /// <summary>
        /// 判断令牌是否存在
        /// </summary>
        /// <param name="token">令牌</param>
        /// <returns></returns>
        bool TokenIsExist(string token);
        /// <summary>
        /// 更新令牌过期时间
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="time">过期时间</param>
        void TokenTimeUpdate(string token, DateTime time);
        /// <summary>
        /// 添加令牌
        /// </summary>
        /// <param name="token">令牌</param>
        /// <param name="info">凭证</param>
        /// <param name="timeout">过期时间</param>
        void TokenInsert(string token, object info, DateTime timeout);
        /// <summary>
        /// 删除令牌
        /// </summary>
        /// <param name="token"></param>
        void DeleteToken(string token);

    }
}
