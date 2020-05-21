using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.FastDFS
{
    public class FDFSConfig
    {
        /// <summary>
        /// 可同时与storage服务器通信的最大连接数
        /// </summary>
        public static int Storage_MaxConnection = 20;
        /// <summary>
        /// 可同时与tracker服务器通信的最大连接数
        /// </summary>
        public static int Tracker_MaxConnection = 10;
        /// <summary>
        /// 与服务器连接的过期时间(单位：秒)
        /// </summary>
        public static int ConnectionTimeout = 5;    //Second
        /// <summary>
        /// 连接池中连接的过期时间(单位：秒)
        /// </summary>
        public static int Connection_LifeTime = 3600;
        /// <summary>
        /// tracker服务器组健康性检测周期(单位：秒)
        /// </summary>
        public static int Tracker_Check_Period = 10;
        public static Encoding Charset = Encoding.UTF8;
    }
}
