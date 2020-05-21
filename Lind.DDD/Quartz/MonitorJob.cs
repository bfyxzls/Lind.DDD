using Lind.DDD.RedisUtils;
using Quartz;
using Quartz.Impl;
using RedisUtils;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Lind.DDD.Quartz
{
    /// <summary>
    /// 监控Job
    /// 监控Job进程的工具情况，主要定时向存储器更新时间戳
    /// </summary>
    public class MonitorJob : JobBase
    {
        protected override void ExcuteJob(IJobExecutionContext context)
        {
            var dic = context.JobDetail.JobDataMap;
            object serviceName, interval, port, url;
            dic.TryGetValue("Url", out url);
            dic.TryGetValue("Port", out port);
            dic.TryGetValue("Interval", out interval);
            dic.TryGetValue("ServiceName", out serviceName);
            var obj = Utils.SerializeMemoryHelper.SerializeToJson(new MonitorModel
            {
                Url = Convert.ToString(url),
                Port = Convert.ToInt32(port),
                Interval = Convert.ToInt32(interval),
                ServiceName = Convert.ToString(serviceName)
            });
            RedisManager.Instance.GetDatabase().HashSet("MonitorJob", serviceName.ToString(), obj);
        }
    }
    public class MonitorModel
    {
        public string Url { get; set; }
        public int Port { get; set; }
        public int Interval { get; set; }
        public string ServiceName { get; set; }
    }
}
