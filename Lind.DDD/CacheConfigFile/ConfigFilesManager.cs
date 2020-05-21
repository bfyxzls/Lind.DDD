using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
namespace Lind.DDD.CacheConfigFile
{
    /// <summary>
    /// 基本文件配置信息管理者
    /// </summary>
    internal class ConfigFilesManager : Singleton<ConfigFilesManager>
    {

        #region 私有

        /// <summary>
        /// 锁对象
        /// </summary>
        object lockHelper = new object();

        /// <summary>
        /// 配置文件修改时间,以文件名为键，修改时间为值
        /// </summary>
        public static Dictionary<string, DateTime> fileChangeTime;

        #endregion

        #region 构造方法
        static ConfigFilesManager()
        {
            fileChangeTime = new Dictionary<string, DateTime>();
        }
        /// <summary>
        /// 私用无参方法，实例单例时用
        /// </summary>
        private ConfigFilesManager()
        {
        }

        #endregion

        #region 配置操作

        #region 加载配置类
        /// <summary>
        /// 从配置文件中读取
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        private IConfiger LoadConfigFile<T>(string fileName)
        {
            fileChangeTime[fileName] = File.GetLastWriteTime(fileName);//得到配置文件的最后修改改时间    
            return ConfigSerialize.DeserializeInfo<T>(fileName);
        }

        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal IConfiger LoadConfig<T>(string fileName)
        {
            return LoadConfig<T>(fileName, true);
        }
        /// <summary>
        /// 加载配置文件
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="type">实体类型</param>
        /// <param name="isCache">是否从缓存加载</param>
        /// <returns></returns>
        internal IConfiger LoadConfig<T>(string fileName, bool isCache)
        {
            if (!isCache)
                return LoadConfigFile<T>(fileName);
            lock (lockHelper)
            {
                if (DataCache.GetCache(fileName) == null)
                    DataCache.SetCache(fileName, LoadConfigFile<T>(fileName));
                DateTime newfileChangeTime = File.GetLastWriteTime(fileName);
                if (!newfileChangeTime.Equals(fileChangeTime[fileName]))
                {
                    DataCache.SetCache(fileName, LoadConfigFile<T>(fileName));
                    return LoadConfigFile<T>(fileName);
                }
                else
                {
                    return DataCache.GetCache(fileName) as IConfiger;
                }
            }
        }
        #endregion
        #endregion

    }
}
