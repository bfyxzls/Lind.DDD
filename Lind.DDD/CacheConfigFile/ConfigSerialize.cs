using System.IO;
using System.Xml.Serialization;
using System;

namespace Lind.DDD.CacheConfigFile
{
    /// <summary>
    /// �������л�������
    /// </summary>
    internal class ConfigSerialize
    {
        #region �����л�ָ������

        /// <summary>
        /// �����л�ָ������
        /// </summary>
        /// <param name="configfilepath">config �ļ���·��</param>
        /// <param name="configtype">��Ӧ������</param>
        /// <returns></returns>
        public static IConfiger DeserializeInfo<T>(string path)
        {
            return Utils.SerializationHelper.DeserializeFromJson<T>(path) as IConfiger;
        }



        #endregion

        #region ����(���л�)ָ��·���µ������ļ�

        /// <summary>
        /// ����(���л�)ָ��·���µ������ļ�
        /// </summary>
        /// <param name="configFilePath">ָ���������ļ����ڵ�·��(�����ļ���)</param>
        /// <param name="configinfo">������(���л�)�Ķ���</param>
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
