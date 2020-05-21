using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;


namespace FastSocket.SocketBase
{
    /// <summary>
    /// 序列化辅助类
    /// </summary>
    public class SerializeHelper
    {

        public static long GetByteSize(object o)
        {
            BinaryFormatter bFormatter = new BinaryFormatter();
            using (MemoryStream stream = new MemoryStream())
            {
                bFormatter.Serialize(stream, o);
                return stream.Length;
            }
        }

        #region Binary
        /// <summary>
        /// 将对象序列化到字节流
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] SerializeToBinary(object value)
        {
            BinaryFormatter serializer = new BinaryFormatter();
            MemoryStream memStream = new MemoryStream();
            memStream.Seek(0, 0);
            serializer.Serialize(memStream, value);
            return memStream.ToArray();
        }
        /// <summary>
        /// 将字节流反序列化成对象
        /// </summary>
        /// <param name="someBytes"></param>
        /// <returns></returns>
        public static object DeserializeFromBinary(byte[] someBytes)
        {
            IFormatter bf = new BinaryFormatter();
            object res = null;
            if (someBytes == null)
                return null;
            using (var memoryStream = new MemoryStream())
            {
                memoryStream.Write(someBytes, 0, someBytes.Length);
                memoryStream.Seek(0, 0);
                memoryStream.Position = 0;
                res = bf.Deserialize(memoryStream);
            }
            return res;

        }


        #endregion

    }
}