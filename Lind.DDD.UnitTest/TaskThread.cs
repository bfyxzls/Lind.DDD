using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class TaskThread
    {
        [TestMethod]
        public void TestParallel()
        {
            int result = 0;
            var actions = new List<Action>();
            actions.Add(() =>
            {
                Thread.Sleep(3000);
                Console.WriteLine("3处理完成");
                result = 3;
            });
            actions.Add(() =>
            {
                Thread.Sleep(2000);
                Console.WriteLine("2处理完成");
                result = 2;
            });
            actions.Add(() =>
            {
                Thread.Sleep(4000);
                Console.WriteLine("4处理完成");
                result = 4;
            });
            Stopwatch sw = new Stopwatch();
            sw.Restart();
            Parallel.Invoke(actions.ToArray());
            sw.Stop();
            Console.WriteLine("result={0},run time={1}", result, sw.ElapsedMilliseconds);

        }
    }
}
