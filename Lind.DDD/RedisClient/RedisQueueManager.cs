using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Lind.DDD.Caching;
using RedisUtils;

namespace Lind.DDD.RedisUtils
{
    /// <summary>
    /// redis队列管理者,简单的队列添加内容，以及实时消费等功能，与Lind.DDD.CachingQueue不同它将会连接到本地的redis服务器 ， 
    /// 默认6379端口,多个连接通过逗号分割 。 其他选项在名称的后面包含了一个 “= ”。 例如
    /// var conn = ConnectionMultiplexer.Connect("redis0:6380,redis1:6380,allowAdmin=true");
    /// </summary>
    public class RedisQueueManager
    {

        /// <summary>
        /// 实时队列消费者
        /// </summary>
        /// <param name="action"></param>
        public static void DoQueue<T>(Action<T> action, string queueName)
        {


            Lind.DDD.Utils.ThreadManager.Run(() =>
            {
                try
                {
                    while (true)
                    {
                        try
                        {
                            if (!RedisManager.Instance.GetDatabase().KeyExists(queueName) || RedisManager.Instance.GetDatabase().ListLength(queueName) == 0) //消息为空挂起
                            {
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("队列{0}为空，挂起1秒", queueName);
                                Thread.Sleep(1000);
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("从队列{0}拿出数据", queueName);

                                var entity = RedisManager.Instance.GetDatabase().PopJson<T>(queueName);
                                if (entity != null)
                                {
                                    Logger.LoggerFactory.Instance.Logger_Info("实时队列：" + queueName);
                                    action(entity);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LoggerFactory.Instance.Logger_Info("实时队列出现异常：" + ex.Message);
                            Thread.Sleep(5000);
                        }


                    }
                }
                catch (Exception ex)
                {
                    Logger.LoggerFactory.Instance.Logger_Info("Redis连接串出现异常：" + ex.Message);
                    Thread.Sleep(5000);
                }

            });
        }
        /// <summary>
        /// 向队列添加对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="queueName"></param>
        /// <param name="obj"></param>
        public static void Push<T>(string queueName, T obj)
        {
            RedisManager.Instance.GetDatabase().Push(queueName, obj);
        }
    }
}
