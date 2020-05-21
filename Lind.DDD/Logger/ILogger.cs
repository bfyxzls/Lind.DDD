using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Logger
{
    /// <summary>
    /// 日志功能接口规范
    /// </summary>
    public interface ILogger
    {
        #region 功能日志
        /// <summary>
        /// 记录代码运行时间，日志文件名以codeTime开头的时间戳
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="action">所测试的代码块</param>
        void Logger_Timer(string message, Action action);
        /// <summary>
        /// 记录代码运行异常，日志文件名以Exception开头的时间戳
        /// </summary>
        /// <param name="message">消息</param>
        /// <param name="action">要添加try...catch的代码块</param>
        void Logger_Exception(string message, Action action);
        #endregion

        #region 级别日志
        /// <summary>
        /// 将message记录到日志文件
        /// </summary>
        /// <param name="message"></param>
        void Logger_Info(string message);
        /// <summary>
        /// 异常发生的日志
        /// </summary>
        /// <param name="message"></param>
        void Logger_Error(Exception ex);
        /// <summary>
        /// 调试期间的日志
        /// </summary>
        /// <param name="message"></param>
        void Logger_Debug(string message);
        /// <summary>
        /// 引起程序终止的日志
        /// </summary>
        /// <param name="message"></param>
        void Logger_Fatal(string message);
        /// <summary>
        /// 引起警告的日志
        /// </summary>
        /// <param name="message"></param>
        void Logger_Warn(string message);
        #endregion

        /// <summary>
        /// 设二级目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        ILogger SetPath(string path);
    }
}
