using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lind.DDD.Logger.Implements
{
    /// <summary>
    /// 以普通的文字流的方式写日志
    /// </summary>
    public class NormalLogger : LoggerBase
    {

        static readonly object objLock = new object();
        protected override void InputLogger(string message)
        {

            if (string.IsNullOrWhiteSpace(FileUrl))
            {
                FileUrl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoggerDir");
            }

            if (!System.IO.Directory.Exists(FileUrl))
                System.IO.Directory.CreateDirectory(FileUrl);

            string filePath = Path.Combine(FileUrl, DateTime.Now.ToLongDateString() + ".log");

            //写日志委托
            Action<string> write = (fileName) =>
            {
                lock (objLock)//防治多线程读写冲突
                {

                    using (System.IO.StreamWriter srFile = new System.IO.StreamWriter(fileName, true))
                    {
                        srFile.WriteLine(string.Format("{0}{1}{2}"
                            , DateTime.Now.ToString().PadRight(20)
                            , ("[TID:" + Thread.CurrentThread.ManagedThreadId.ToString().PadLeft(3, '0') + "]").PadRight(10)
                            , message));


                    }
                }
            };

            try
            {
                write(filePath);
            }
            catch (Exception)
            {
                write(Path.Combine(FileUrl, DateTime.Now.ToLongDateString() + Process.GetCurrentProcess().Id + ".log"));
            }
        }

    }
}
