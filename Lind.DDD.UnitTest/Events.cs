using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

using Lind.DDD.Events;
using Lind.DDD.Events.Demo;
namespace Lind.DDD.UnitTest
{
    /// <summary>
    /// 事件总线
    /// </summary>
    [TestClass]
    public class Events
    {
        [TestMethod]
        public void SendSMS()
        {

            EventBus.Instance.SubscribeAll();
            EventBus.Instance.Publish(new OrderEvent()
            {
                OrderId = Guid.NewGuid(),
                Title = "test",
                UserEmailAddress = "zzl@sina.com",
                ConfirmedDate = DateTime.Now,
            });
        }
    }
}
