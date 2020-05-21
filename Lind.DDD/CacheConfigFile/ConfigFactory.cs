using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using System.IO;
using System.Web;
using System.Configuration;

namespace Lind.DDD.CacheConfigFile
{
    /// <summary>
    /// 配置信息生产工厂
    /// </summary>
    public class ConfigFactory : Singleton<ConfigFactory>
    {
        private ConfigFactory()
        {

        }

        #region 公开的属性
        /// <summary>
        /// 得到ＷＥＢ网站下的指定文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public T GetConfig<T>() where T : IConfiger, new()
        {
            return GetConfig<T>(null);
        }
        /// <summary>
        /// 可以根据绝对路径得到文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configPath">配置文件的完整路径</param>
        /// <returns></returns>
        public T GetConfig<T>(string configPath) where T : IConfiger, new()
        {
            string configFilePath = configPath;
            string filename = typeof(T).Name;

            if (string.IsNullOrWhiteSpace(configFilePath))
                configFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, string.Format(@"Configs\\{0}.json", filename));

            if (!File.Exists(configFilePath))
            {
                Directory.CreateDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs"));
                ConfigSerialize.Serializer(configFilePath, new T());
                Logger.LoggerFactory.Instance.Logger_Warn("配置文件第一次被建立,文件路径" + configFilePath + "请手动修改相关配置项");
                throw new Exception("配置文件第一次被建立,文件路径" + configFilePath + "请手动修改相关配置项");
            }


            return (T)ConfigFilesManager.Instance.LoadConfig<T>(configFilePath);
        }
        #endregion

    }
}
