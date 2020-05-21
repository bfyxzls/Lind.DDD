using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 分布式主键生产者
    /// 占12个字节的空间，由24个16进制数组成
    /// </summary>
    public class DistributedPrimaryKey
    {
        /// <summary>
        /// The epoch.
        /// </summary>
        private static readonly DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        /// <summary>
        /// The inclock.
        /// </summary>
        private static readonly object inclock = new object();

        /// <summary>
        /// The inc.
        /// </summary>
        private static int inc;

        /// <summary>
        /// The machine hash.
        /// </summary>
        private static byte[] machineHash;

        /// <summary>
        /// The proc id.
        /// </summary>
        private static byte[] procID;

        /// <summary>
        /// Initializes static members of the <see cref="ObjectIdGenerator"/> class. 
        /// </summary>
        static DistributedPrimaryKey()
        {
            GenerateConstants();
        }

        /// <summary>
        /// 生成一个24个数字组成的字符型主键，占用12个字符的存储空间
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> that represents this instance.
        /// </returns>
        public static string NewID()
        {
            return BitConverter.ToString(Generate()).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// Generates a byte array ObjectId.
        /// </summary>
        /// <returns>
        /// </returns>
        private static byte[] Generate()
        {
            var oid = new byte[12];
            var copyidx = 0;
            //时间差
            Array.Copy(BitConverter.GetBytes(GenerateTime()), 0, oid, copyidx, 4);
            copyidx += 4;
            //机器码
            Array.Copy(machineHash, 0, oid, copyidx, 3);
            copyidx += 3;
            //进程码
            Array.Copy(procID, 0, oid, copyidx, 2);
            copyidx += 2;
            //自增值
            Array.Copy(BitConverter.GetBytes(GenerateInc()), 0, oid, copyidx, 3);
            return oid;
        }

        /// <summary>
        /// Generates time.
        /// </summary>
        /// <returns>
        /// The time.
        /// </returns>
        private static int GenerateTime()
        {
            var now = DateTime.Now.ToUniversalTime();

            var nowtime = new DateTime(epoch.Year, epoch.Month, epoch.Day, now.Hour, now.Minute, now.Second, now.Millisecond);
            var diff = nowtime - epoch;
            return Convert.ToInt32(Math.Floor(diff.TotalMilliseconds));
        }

        /// <summary>
        /// Generate an increment.
        /// </summary>
        /// <returns>
        /// The increment.
        /// </returns>
        private static int GenerateInc()
        {
            lock (inclock)
            {
                return inc++;
            }
        }

        /// <summary>
        /// Generates constants.
        /// </summary>
        private static void GenerateConstants()
        {
            machineHash = GenerateHostHash();
            procID = BitConverter.GetBytes(GenerateProcId());
        }

        /// <summary>
        /// Generates a host hash.
        /// </summary>
        /// <returns>
        /// </returns>
        private static byte[] GenerateHostHash()
        {
            using (var md5 = MD5.Create())
            {
                var host = Dns.GetHostName();
                return md5.ComputeHash(Encoding.Default.GetBytes(host));
            }
        }

        /// <summary>
        /// Generates a proc id.
        /// </summary>
        /// <returns>
        /// Proc id.
        /// </returns>
        private static int GenerateProcId()
        {
            var proc = Process.GetCurrentProcess();
            return proc.Id;
        }
    }
}
