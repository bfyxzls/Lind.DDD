using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 配置config文件读取类
    /// </summary>
    public class WebConfig
    {
        #region 获取webconfig中AppSettings设置的参数
        /// <summary>
        /// 获取webconfig中的参数
        /// </summary>
        /// <param name="strKey"></param>
        /// <param name="strDefault"></param>
        /// <returns></returns>
        public static string GetWebConfig(string strKey, string strDefault)
        {
            if (System.Configuration.ConfigurationManager.AppSettings[strKey] == null)
            {
                return strDefault;
            }
            else
            {
                return System.Configuration.ConfigurationManager.AppSettings[strKey].ToString();
            }
        }
        #endregion
    }
}
