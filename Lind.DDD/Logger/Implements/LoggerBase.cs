using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Logger.Implements
{
    /// <summary>
    /// 日志核心基类
    /// 模版方法模式，对InputLogger开放，对其它日志逻辑隐藏，InputLogger可以有多种实现
    /// </summary>
    public abstract class LoggerBase : ILogger
    {

        /// <summary>
        /// 每个子类初始时都执行基类这个构造，初始化当前路径
        /// </summary>
        public LoggerBase()
        {
            FileUrl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoggerDir");
        }

        /// <summary>
        /// 日志文件地址
        /// 优化级为mvc方案地址，网站方案地址，console程序地址
        /// </summary>
        [ThreadStatic]
        static protected string FileUrl;

        /// <summary>
        /// 日志持久化的方法，派生类必须要实现自己的方式
        /// </summary>
        /// <param name="message"></param>
        protected abstract void InputLogger(string message);


        #region ILogger 成员

        public void Logger_Timer(string message, Action action)
        {
            StringBuilder str = new StringBuilder();
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            str.Append(message);
            action();
            str.Append("Logger_Timer:代码段运行时间(" + sw.ElapsedMilliseconds + "毫秒)");
            InputLogger(str.ToString());
            sw.Stop();
        }

        public void Logger_Exception(string message, Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                InputLogger("Logger_Exception:" + message + "代码段出现异常,信息为" + ex.Message);
            }
        }

        /// <summary>
        /// 占位符
        /// </summary>
        const int LEFTTAG = 7;

        public virtual void Logger_Info(string message)
        {

            message = "[Info]".PadLeft(LEFTTAG) + " " + message;
            InputLogger(message);
            Trace.WriteLine(message);
        }

        public virtual void Logger_Error(Exception ex)
        {

            string message = "[Error]".PadLeft(LEFTTAG) + " " + ex.Message;
            InputLogger(message);
            Trace.WriteLine(message);

        }

        public virtual void Logger_Debug(string message)
        {

            message = "[Debug]".PadLeft(LEFTTAG) + " " + message;
            InputLogger(message);
            Trace.WriteLine(message);

        }

        public virtual void Logger_Fatal(string message)
        {
            message = "[Fatal]".PadLeft(LEFTTAG) + " " + message;
            InputLogger(message);
            Trace.WriteLine(message);
        }

        public virtual void Logger_Warn(string message)
        {

            message = "[Warn]".PadLeft(LEFTTAG) + " " + message;
            InputLogger(message);
            Trace.WriteLine(message);

        }

        public ILogger SetPath(string path)
        {
            if (!string.IsNullOrWhiteSpace(path))
            {
                FileUrl = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "LoggerDir", path);
            }
            return this;
        }


        #endregion
    }
}
