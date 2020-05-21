using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 线程管理
    /// </summary>
    public class ThreadManager
    {
        /// <summary>
        /// 将在线程池上运行的指定工作排队
        /// </summary>
        /// <param name="action"></param>
        public static void Run(Action action)
        {
            ThreadPool.QueueUserWorkItem(u =>
            {
                try
                {
                    action();
                }
                catch (Exception ex)
                {
                    Lind.DDD.Logger.LoggerFactory.Instance.Logger_Error(ex);
                }
            });

        }
        /// <summary>
        /// 将在线程池上运行的指定工作排队
        /// </summary>
        /// <param name="callBack"></param>
        public static void Run(WaitCallback callBack)
        {
            ThreadPool.QueueUserWorkItem(u =>
            {
                try
                {
                    callBack(null);
                }
                catch (Exception ex)
                {
                    Lind.DDD.Logger.LoggerFactory.Instance.Logger_Error(ex);
                }
            });

        }
    }
}
