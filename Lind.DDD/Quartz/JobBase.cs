using MongoDB.Bson;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Lind.DDD.Quartz
{
    /// <summary>
    /// 工作任务基类
    /// DisallowConcurrentExecution:禁止单job并发执行，当job周期短，job运行时间长时，使用这个选项
    /// </summary>
    [DisallowConcurrentExecution()]
    public abstract class JobBase : IJob
    {
        /// <summary>
        /// log4日志对象
        /// </summary>
        protected log4net.ILog Logger
        {
            get
            {
                return log4net.LogManager.GetLogger(this.GetType());//得到当前类类型（当前实实例化的类为具体子类）
            }
        }

        #region IJob 成员

        public void Execute(IJobExecutionContext context)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Red;
                ExcuteJob(context);
                Console.WriteLine(DateTime.Now.ToString() + "{0}这个Job开始执行", context.JobDetail.Key.Name);

            }
            catch (Exception ex)
            {
                Lind.DDD.Logger.LoggerFactory.Instance.Logger_Debug(this.GetType().Name + "error:" + ex.Message);
                throw;
            }
        }

        #endregion

        /// <summary>
        /// Job具体类去实现自己的逻辑
        /// </summary>
        protected abstract void ExcuteJob(IJobExecutionContext context);
    }
}

