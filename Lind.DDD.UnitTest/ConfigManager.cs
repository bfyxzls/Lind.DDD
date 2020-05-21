using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class ConfigManager
    {
        [TestMethod]
        public void ModifyFile()
        {
            Console.WriteLine(ConfigConstants.ConfigManager.Config.Logger.Type);
            Console.WriteLine(ConfigConstants.ConfigManager.Config.Logger.Type);

        }
    }
}
