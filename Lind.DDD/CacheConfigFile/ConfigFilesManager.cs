using System;
using System.Web;
using System.IO;
using System.Collections.Generic;
namespace Lind.DDD.CacheConfigFile
{
    /// <summary>
    /// �����ļ�������Ϣ������
    /// </summary>
    internal class ConfigFilesManager : Singleton<ConfigFilesManager>
    {

        #region ˽��

        /// <summary>
        /// ������
        /// </summary>
        object lockHelper = new object();

        /// <summary>
        /// �����ļ��޸�ʱ��,���ļ���Ϊ�����޸�ʱ��Ϊֵ
        /// </summary>
        public static Dictionary<string, DateTime> fileChangeTime;

        #endregion

        #region ���췽��
        static ConfigFilesManager()
        {
            fileChangeTime = new Dictionary<string, DateTime>();
        }
        /// <summary>
        /// ˽���޲η�����ʵ������ʱ��
        /// </summary>
        private ConfigFilesManager()
        {
        }

        #endregion

        #region ���ò���

        #region ����������
        /// <summary>
        /// �������ļ��ж�ȡ
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="type"></param>
        private IConfiger LoadConfigFile<T>(string fileName)
        {
            fileChangeTime[fileName] = File.GetLastWriteTime(fileName);//�õ������ļ�������޸ĸ�ʱ��    
            return ConfigSerialize.DeserializeInfo<T>(fileName);
        }

        /// <summary>
        /// ���������ļ�
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal IConfiger LoadConfig<T>(string fileName)
        {
            return LoadConfig<T>(fileName, true);
        }
        /// <summary>
        /// ���������ļ�
        /// </summary>
        /// <param name="fileName">�ļ���</param>
        /// <param name="type">ʵ������</param>
        /// <param name="isCache">�Ƿ�ӻ������</param>
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
