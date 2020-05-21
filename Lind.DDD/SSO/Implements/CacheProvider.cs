using Lind.DDD.ConfigConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Lind.DDD.SSO.Implements
{
    internal class CacheProvider : ISSOProvider
    {

        /// <summary>
        /// 初始化数据结构
        /// </summary>
        /// <remarks>
        /// ----------------------------------------------------
        /// | token(令牌) | Certificate(用户凭证) | timeout(过期时间) |
        /// |--------------------------------------------------|
        /// </remarks>
        private static void cacheInit()
        {
            if (HttpContext.Current.Cache[ConfigManager.Config.SSO.SSOKey] == null)
            {
                //Cache的过期时间为 令牌过期时间*2
                HttpContext.Current.Cache.Insert(ConfigManager.Config.SSO.SSOKey, new List<CertificationModel>(), null, DateTime.MaxValue, TimeSpan.FromMinutes(10 * 2));
            }
        }

        #region ISSOProvider 成员

        public bool TokenIsExist(string token)
        {
            cacheInit();
            var sso = HttpContext.Current.Cache[ConfigManager.Config.SSO.SSOKey] as List<CertificationModel>;
            return sso.FirstOrDefault(i => i.Token == token) != null;
        }

        public void TokenTimeUpdate(string token, DateTime time)
        {
            cacheInit();

            var sso = HttpContext.Current.Cache[ConfigManager.Config.SSO.SSOKey] as List<CertificationModel>;
            var entity = sso.FirstOrDefault(i => i.Token == token);
            if (entity != null)
            {
                entity.Expire = time;
            }
        }

        public void TokenInsert(string token, object info, DateTime timeout)
        {
            cacheInit();

            if (!TokenIsExist(token))
            {
                var sso = HttpContext.Current.Cache[ConfigManager.Config.SSO.SSOKey] as List<CertificationModel>;
                sso.Add(new CertificationModel
                {
                    Token = token,
                    Certificate = info,
                    Expire = timeout,
                });
                HttpContext.Current.Cache[ConfigManager.Config.SSO.SSOKey] = sso;
            }
            else
            {
                TokenTimeUpdate(token, timeout);
            }
        }

        public List<CertificationModel> GetCacheTable()
        {
            List<CertificationModel> dt = null;
            if (HttpContext.Current.Cache[ConfigManager.Config.SSO.SSOKey] != null)
            {
                dt = HttpContext.Current.Cache[ConfigManager.Config.SSO.SSOKey] as List<CertificationModel>;
            }
            return dt;
        }

        public void DeleteToken(string token)
        {
            var sso = GetCacheTable();
            if (sso != null)
            {
                var entity = sso.FirstOrDefault(i => i.Token == token);
                if (entity != null)
                {
                    sso.Remove(entity);
                }
            }
        }

        #endregion
    }
}
