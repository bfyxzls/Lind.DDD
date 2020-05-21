using Lind.DDD.ConfigConstants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using RedisUtils;

namespace Lind.DDD.SSO.Implements
{
    internal class RedisProvider : ISSOProvider
    {


        #region ISSOProvider 成员

        public List<CertificationModel> GetCacheTable()
        {
            if (!RedisManager.Instance.GetDatabase().KeyExists(ConfigManager.Config.SSO.SSOKey))
                RedisManager.Instance.GetDatabase().Set(ConfigManager.Config.SSO.SSOKey, new List<CertificationModel>());
            return RedisManager.Instance.GetDatabase().Get<List<CertificationModel>>(ConfigManager.Config.SSO.SSOKey);
        }

        public bool TokenIsExist(string token)
        {
            return GetCacheTable().Where(i => i.Token == token).Count() > 0;
        }

        public void TokenTimeUpdate(string token, DateTime time)
        {
            var sso = GetCacheTable();
            var entity = sso.FirstOrDefault(i => i.Token == token);
            if (entity != null)
            {
                entity.Expire = time;
            }
            RedisManager.Instance.GetDatabase().Set(ConfigManager.Config.SSO.SSOKey, sso);
        }

        public void TokenInsert(string token, object info, DateTime timeout)
        {
            if (!TokenIsExist(token))
            {
                var sso = GetCacheTable();
                sso.Add(new CertificationModel
                {
                    Token = token,
                    Certificate = info,
                    Expire = timeout,
                });
                RedisManager.Instance.GetDatabase().Set(ConfigManager.Config.SSO.SSOKey, sso);
            }
            else
            {
                TokenTimeUpdate(token, timeout);
            }
        }

        public void DeleteToken(string token)
        {
            var sso = GetCacheTable();
            var entity = sso.FirstOrDefault(i => i.Token == token);
            if (entity != null)
            {
                sso.Remove(entity);
            }
            RedisManager.Instance.GetDatabase().Set(ConfigManager.Config.SSO.SSOKey, sso);
        }

        #endregion
    }
}
