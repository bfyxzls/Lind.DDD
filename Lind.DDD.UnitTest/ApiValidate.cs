using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class ApiValidate
    {
        [TestMethod]
        public void PassKey()
        {
            var passKey = "lind123";
            var client = Lind.DDD.Utils.Encryptor.Utility.EncryptString(passKey + DateTime.Now.ToUniversalTime().ToString("yyyyMMddHHmm"), Utils.Encryptor.Utility.EncryptorType.MD5);
            Console.WriteLine(client);
            var dt = DateTime.UtcNow.AddMinutes(-5);
            var dtEnd = DateTime.UtcNow.AddMinutes(5);
            while (dt <= dtEnd)
            {
                var server = Lind.DDD.Utils.Encryptor.Utility.EncryptString(passKey + dt.ToString("yyyyMMddHHmm"), Utils.Encryptor.Utility.EncryptorType.MD5);
                Console.WriteLine("时间的时：" + dt + "服务端加密：" + server);
                dt = dt.AddMinutes(1);
            }

            var timespan1 = (DateTime.Now.AddMinutes(-1) - DateTime.MinValue).TotalSeconds;
            var timespan2 = (DateTime.Now - DateTime.MinValue).TotalSeconds;
            Console.WriteLine(timespan1 + "," + timespan2);
        }
    }
}
