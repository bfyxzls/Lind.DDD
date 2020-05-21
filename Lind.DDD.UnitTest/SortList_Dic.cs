using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Lind.DDD.UnitTest
{
    public class Test
    {
        public string Name { get; set; }
    }
    [TestClass]
    public class SortList_Dic
    {
        [TestMethod]
        public void TestSortList()
        {
            //键为引用类型
            Dictionary<Test, string> dic = new Dictionary<Test, string>();
            dic.Add(new Test { Name = "1" }, "1");
            dic.Add(new Test { Name = "2" }, "2");


            SortedList<int, string> sl = new SortedList<int, string>();
            sl.Add(2, "zhang");
            sl.Add(0, "zhan");
            sl.Add(1, "ling");
            foreach (var item in sl)
            {
                Console.WriteLine(item.Key + "=" + item.Value);
            }
        }
        [TestMethod]
        public void TestSortDic()
        {
            SortedDictionary<int, string> sl = new SortedDictionary<int, string>();
            sl.Add(2, "zhang");
            sl.Add(0, "hang");
            sl.Add(1, "zheng");
            foreach (var item in sl)
            {
                Console.WriteLine(item.Key + "=" + item.Value);
            }
        }
    }
}
