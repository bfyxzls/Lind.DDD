using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web.Script.Serialization;
using System.Xml.Serialization;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 序列化与反序列化到文件
    /// </summary>
    public class SerializationHelper
    {
        private static object lockObj = new object();

        #region Binary
        /// <summary>
        /// 二进制序列化到磁盘
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        public static void SerializableToBinary(string fileName, object obj)
        {
            lock (lockObj)
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    formatter.Serialize(fs, obj);
                }
            }
        }
        /// <summary>
        /// 二进制反序列化从磁盘到内存对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static object DeserializeFromBinary(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    return formatter.Deserialize(fs);
                }
            }
            catch (Exception)
            {
                return null;
            }

        }
        #endregion

        #region XML
        /// <summary>
        /// XML将对象序列化到磁盘文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        public static void SerializeToXml(string fileName, object obj)
        {
            lock (lockObj)
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    new XmlSerializer(obj.GetType()).Serialize(fs, obj);
                }
            }
        }
        /// <summary>
        /// XML反序列化从磁盘到内存对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static object DeserializeFromXml(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    return new XmlSerializer(typeof(object)).Deserialize(fs);
                }
            }
            catch (Exception)
            {
                return null;
            }

        }

        /// <summary>
        /// 泛型版本：XML将对象序列化到磁盘文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        public static void SerializeToXml<T>(string fileName, T obj) where T : class
        {
            try
            {
                lock (lockObj)
                {
                    using (FileStream fs = new FileStream(fileName, FileMode.Create))
                    {
                        new XmlSerializer(typeof(T)).Serialize(fs, obj);
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

        }
        /// <summary>
        /// 泛型版本：XML反序列化从磁盘到内存对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeserializeFromXml<T>(string fileName) where T : class
        {
            try
            {
                if (!File.Exists(fileName))
                    return null;
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    return new XmlSerializer(typeof(T)).Deserialize(fs) as T;
                }
            }
            catch (System.InvalidOperationException)
            {
                return null;
            }
            catch (Exception)
            {
                throw;
            }
        }


        #endregion

        #region JSON
        /// <summary>
        /// 二进制序列化到磁盘
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="obj"></param>
        public static void SerializableToJson(string fileName, object obj)
        {
            lock (lockObj)
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {
                    using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8))
                    {
                        sw.Write(Newtonsoft.Json.JsonConvert.SerializeObject(obj));
                    }
                }
            }
        }
        /// <summary>
        /// 二进制反序列化从磁盘到内存对象
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static T DeserializeFromJson<T>(string fileName)
        {
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.OpenOrCreate))
                {

                    using (StreamReader sw = new StreamReader(fs, Encoding.UTF8))
                    {
                        return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(sw.ReadToEnd());
                    }

                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }

}
