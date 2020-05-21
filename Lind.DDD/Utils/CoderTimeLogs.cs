using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace Lind.DDD.Utils
{
    #region 日志

    /// <summary>
    /// 日志操作规范
    /// </summary>
    public interface ILog
    {
        void Write(string msg);
    }
    /// <summary>
    /// 日志格式规范
    /// </summary>
    public interface ILogFormatter
    {
        string Format(string msg);
    }
    /// <summary>
    /// 实现一个文本日志
    /// </summary>
    public class TextLog : ILog
    {
        #region Fields
        ILogFormatter _iLogFormatter;
        string _filePath;
        string _message;
        #endregion

        #region Constructs
        public TextLog(string message, ILogFormatter iLogFormatter, string filePath)
        {
            _iLogFormatter = iLogFormatter;
            _filePath = filePath;
            _message = message;
        }
        public TextLog(string message, ILogFormatter iLogFormatter)
            : this(message, iLogFormatter, string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "TextLog.log"))
        { }
        public TextLog(string message)
            : this(message, new FormatStandard())
        { }
        public TextLog()
            : this(null, new FormatStandard())
        { }
        public TextLog(ILogFormatter iLogFormatter)
            : this(null, iLogFormatter)
        { }
        public TextLog(ILogFormatter iLogFormatter, string filePath)
            : this(null, iLogFormatter, filePath)
        { }
        #endregion

        #region ILog 成员

        public void Write(string msg)
        {
            StringBuilder sb = new StringBuilder();
            if (!string.IsNullOrWhiteSpace(_message))
                sb.Append(_message);
            sb.Append(_iLogFormatter.Format(msg));
            System.IO.File.AppendAllText(_filePath, sb.ToString(), Encoding.UTF8);
        }

        #endregion
    }
    /// <summary>
    /// 实现一个标准的格式
    /// </summary>
    public class FormatStandard : ILogFormatter
    {
        #region ILogFormatter 成员

        public string Format(string msg)
        {
            return string.Format("\r\n发生时间：[{0}]\r\n详细信息:[{1}]\r\n", DateTime.Now, msg);
        }

        #endregion
    }

    #endregion

    /// <summary>
    /// 代码运行时间记录
    /// author:张占岭
    /// create date:2012-1-10
    /// </summary>
    public class CoderTimeLogs
    {
        public static void CodeRunTimeLogs(ILog iLog, Action action)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Restart();
            action();
            stopwatch.Stop();
            iLog.Write("运行时间为：" + stopwatch.ElapsedMilliseconds + "豪秒");

        }
    }
}
