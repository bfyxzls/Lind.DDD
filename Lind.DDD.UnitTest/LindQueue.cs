using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Utils;
using System.Linq;
using System.Collections.Generic;
using Lind.DDD.LindMQ;
namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class LindQueue
    {
        [TestMethod]
        public void GetQueueOffset()
        {
            foreach (var item in BrokerManager.GetQueueOffset())
            {
                Console.WriteLine(item.Key + "-" + item.Value);
            }
        }

        [TestMethod]
        public void PUSH()
        {

            var pm = new ProducerManager(new ProducerSetting
            {
                BrokerAddress = "127.0.0.1",
                BrokerName = "test",
                BrokerPort = 8406,
                Timeout = 1000,
            });

            for (int i = 0; i < 10; i++)
            {
                pm.Push(new MessageBody
                {
                    Topic = "zhz",
                    Body = Utils.SerializeMemoryHelper.SerializeToJson(new { Name = "zzl", SortNumber = i })
                });

                pm.Push(new MessageBody
                {
                    Topic = "zzl",
                    Body = Utils.SerializeMemoryHelper.SerializeToJson(new { Name = "zzl", SortNumber = i, Email = "bfyxzls2sina.com" })
                });

                pm.Push(new MessageBody
                {
                    Topic = "order_Paid",
                    Body = Utils.SerializeMemoryHelper.SerializeToJson(new { UserName = "zzl", Age = i })
                });
            }



        }

        [TestMethod]
        public void RemoveQueue()
        {
            BrokerManager.AutoRemoveQueue();
        }

        [TestMethod]
        public void DisplayQueue()
        {

            foreach (var item in BrokerManager.GetMessageBody("zzl"))
            {
                Console.WriteLine(item.ToString());
            }
        }

    }
}
