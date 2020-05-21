using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 关于GUID码的扩展功能
    /// </summary>
    public class GuidHelper
    {
        /// <summary>
        /// 得到GUID码的字符串
        /// </summary>
        /// <returns></returns>
        public static string GetGuidString()
        {
            return Guid.NewGuid().ToString();
        }
        /// <summary>
        /// 得到GUID码的长整形结构
        /// 后话：原来BitConverter.ToInt64方法，只取buffer从startIndex开始向后加7个字节的值。
        /// 也就是说，我们16字节的高8个字节被忽略掉了。GUID理想情况下，要2^128个数据才会出现冲突，
        /// 而转换后，把字节数减半，也就是2^64数据就会出现冲突。
        /// </summary>
        /// <returns></returns>
        public static long GetGuidLong()
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            return BitConverter.ToInt64(buffer, 0);
        }
    }
}
