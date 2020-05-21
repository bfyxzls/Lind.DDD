using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class FlagMax
    {
        [TestMethod]
        public void Max()
        {
            int c = 0;
            for (long i = 2; i < 2048; i = i << 1)
            {
                c++;
                Console.WriteLine("i<<" + c + "=" + i);
            }
        }
    }
}
