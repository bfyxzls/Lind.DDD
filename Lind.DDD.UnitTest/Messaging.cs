using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Messaging;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class Messaging
    {
        [TestMethod]
        public void Email()
        {
            Lind.DDD.Messaging.MessageFactory.GetService(MessageType.Email).Send("853066980@qq.com", "test", "test");
        }
    }
}
