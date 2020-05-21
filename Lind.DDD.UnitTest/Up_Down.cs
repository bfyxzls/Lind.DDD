using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Lind.DDD.Domain;
using Lind.DDD.Utils;

namespace Lind.DDD.UnitTest
{
    public class Person : Lind.DDD.Domain.Entity, ISortBehavor
    {
        public string Name { get; set; }

        public int SortNumber
        {
            get;
            set;
        }
    }

    /// <summary>
    /// 排序：上移和下移
    /// </summary>
    [TestClass]
    public class Up_Down
    {
        [TestMethod]
        public void TestMethod1()
        {
            List<Person> list = new List<Person>();
            list.Add(new Person { Id = 1, Name = "zzl", SortNumber = 1 });
            list.Add(new Person { Id = 2, Name = "zzl2", SortNumber = 2 });
            list.Add(new Person { Id = 3, Name = "zzl3", SortNumber = 3 });
            list.Add(new Person { Id = 4, Name = "zzl4", SortNumber = 4 });
            list.Add(new Person { Id = 5, Name = "zzl5", SortNumber = 5 });
            list.Add(new Person { Id = 6, Name = "zzl6", SortNumber = 6 });
            list.Add(new Person { Id = 7, Name = "zzl7", SortNumber = 7 });
            list.Add(new Person { Id = 8, Name = "zzl8", SortNumber = 8 });
            list.Add(new Person { Id = 9, Name = "zzl9", SortNumber = 9 });
            Console.WriteLine("Hello World!");
            AlgorithmsHelper.Sortable_Up_Down(list, 1,7);
        }
    }
}
