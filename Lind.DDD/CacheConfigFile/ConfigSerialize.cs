using System.IO;
using System.Xml.Serialization;
using System;

namespace Lind.DDD.CacheConfigFile
{
    /// <summary>
    /// 配置序列化操作类
    /// </summary>
    internal class ConfigSerialize
    {
        #region 反序列化指定的类

        /// <summary>
        /// 反序列化指定的类
        /// </summary>
        /// <param name="configfilepath">config 文件的路径</param>
        /// <param name="configtype">相应的类型</param>
        /// <returns></returns>
        public static IConfiger DeserializeInfo<T>(string path)
        {
            return Utils.SerializationHelper.DeserializeFromJson<T>(path) as IConfiger;
        }



        #endregion

        #region 保存(序列化)指定路径下的配置文件

        /// <summary>
        /// 保存(序列化)指定路径下的配置文件
        /// </summary>
        /// <param name="configFilePath">指定的配置文件所在的路径(包括文件名)</param>
        /// <param name="configinfo">被保存(序列化)的对象</param>
        /// <returns></returns>
        public static bool Serializer(string path, IConfiger Iconfiginfo)
        {

            bool succeed = false;
            try
            {
                Utils.SerializationHelper.SerializableToJson(path, Iconfiginfo);
                succeed = true;
            }
            catch (Exception)
            {

                throw;
            }

            return succeed;
        }

        #endregion

    }
}
