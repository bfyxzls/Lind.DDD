using Lind.DDD.RedisUtils;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Quartz
{
    public class demo
    {
        static void Main(string[] args)
        {
            NameValueCollection properties = new NameValueCollection();

            // 远程输出配置
            properties["quartz.scheduler.exporter.type"] = "Quartz.Simpl.RemotingSchedulerExporter, Quartz";
            properties["quartz.scheduler.exporter.port"] = "556";
            properties["quartz.scheduler.exporter.bindName"] = "QuartzScheduler";
            properties["quartz.scheduler.exporter.channelType"] = "tcp";

            properties["quartz.scheduler.instanceName"] = "XmlConfiguredInstance";

            // set thread pool info
            properties["quartz.threadPool.type"] = "Quartz.Simpl.SimpleThreadPool, Quartz";
            properties["quartz.threadPool.threadCount"] = "6";
            properties["quartz.threadPool.threadPriority"] = "Normal";

            // job initialization plugin handles our xml reading, without it defaults are used
            properties["quartz.plugin.xml.type"] = "Quartz.Plugin.Xml.XMLSchedulingDataProcessorPlugin, Quartz";
            properties["quartz.plugin.xml.fileNames"] = "E:\\MicrosoftTFS\\慢查询任务机制\\Test\\quartz_jobs.xml";

            //配置文件修改后，需要重启Quartz服务
            ISchedulerFactory sf = new StdSchedulerFactory(properties);
            IScheduler sched = sf.GetScheduler();
            sched.Start();

            RedisQueueManager.DoQueue<string>((msg) => { Console.WriteLine(msg); }, "test");



        }
    }
}
