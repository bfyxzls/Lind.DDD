using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Lind.DDD.Logger;
namespace Lind.DDD.UnitTest
{
    /// <summary>
    /// 日志
    /// </summary>
    [TestClass]
    public class Logger
    {
        [TestMethod]
        public void TestMethod1()
        { 
            LoggerFactory.Instance.Logger_Info("test hello");   
        }
    }
}
