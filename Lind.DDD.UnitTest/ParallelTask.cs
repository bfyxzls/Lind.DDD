using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class ParallelTask
    {
        [TestMethod]
        public void TestMethod1()
        {
            ConcurrentList<int> intList = new ConcurrentList<int>();
            var result = Parallel.ForEach(Enumerable.Range(1, 10000), (val) =>
            {
                intList.Add(val);
            });
            if (result.IsCompleted)
            {
                Console.WriteLine("intList.Count():" + intList.Count);
            }
        }

        [TestMethod]
        public void TestMethod0()
        {
            List<int> intList = new List<int>();
            var result = Parallel.ForEach(Enumerable.Range(1, 10000), (val) =>
            {
                intList.Add(val);
            });
            if (result.IsCompleted)
            {
                Console.WriteLine("intList.Count():" + intList.Count);
            }
        }
    }
}
