using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Lind.DDD.UnitTest
{
    enum AUDIT
    {
        READ = 1,
        WRITE = READ << 1,
        DELETE = READ << 2,
        SORT = READ << 3,
        FREEZE = READ << 4,
        AUDIT = READ << 5,
        NOTAUDIT = READ << 6,
        NOTFREEZE = READ << 7,
        DISPLAY = READ << 8,
        HIDDEN = READ << 9,
        OK = READ << 10,
        CANCLE = READ << 11,
        CLOSE = READ << 12,
        OPEN = READ << 13,
    }
    [TestClass]
    public class EnumTest
    {
        [TestMethod]
        public void EnumTestMethod1()
        {
            foreach (var item in Enum.GetValues(typeof(AUDIT)))
            {
                Console.WriteLine(item + "=" + (int)item);
            }
        }
    }
}
