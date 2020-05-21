using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace Lind.DDD.UnitTest
{
    /// <summary>
    /// 具有排序功能的接口
    /// </summary>
    public interface ISortable
    {
        int SortNumber { get; set; }
    }
    [Serializable]
    public class People : Lind.DDD.Domain.Entity, ISortable
    {
        public People()
        {

        }
        public int Id { get; set; }
        public string Name { get; set; }
        public int SortNumber { get; set; }
        public People(int id, string name, int sortNumber)
        {
            this.Id = id;
            this.Name = name;
            this.SortNumber = sortNumber;
        }
    }
    [TestClass]
    public class MoveSortFromSet
    {
        List<People> list = new List<People>();
        public MoveSortFromSet()
        {
            list.Add(new People(1, "zzl1", 1));
            list.Add(new People(2, "zzl2", 2));
            list.Add(new People(3, "zzl2", 3));
            list.Add(new People(4, "zzl4", 4));
            list.Add(new People(5, "zzl5", 5));
            list.Add(new People(6, "zzl6", 6));
            list.Add(new People(7, "zzl7", 7));
            list.Add(new People(8, "zzl8", 8));
            list.Add(new People(9, "zzl9", 9));
        }
        void swap<T>(List<T> list, int oldId, int newSort)
        {

            var old = list.Find(i => (i as Lind.DDD.Domain.Entity).Id == oldId) as ISortable;

            var old2 = list.Find(i => (i as Lind.DDD.Domain.Entity).Id == oldId);
            if (old.SortNumber == newSort)
                return;

            if (old.SortNumber > newSort)
            {
                old.SortNumber = newSort;
                foreach (ISortable item in list.FindAll(i => (i as ISortable).SortNumber >= newSort && (i as Lind.DDD.Domain.Entity).Id != oldId))
                {
                    int newsort = (list.Find(j => (j as ISortable).SortNumber == (item as ISortable).SortNumber) as ISortable).SortNumber + 1;
                    item.SortNumber = newsort;
                }
            }
            else
            {
                old.SortNumber = newSort;
                foreach (ISortable item in list.FindAll(i => (i as ISortable).SortNumber <= newSort && (i as Lind.DDD.Domain.Entity).Id != oldId))
                {
                    int newsort = (list.Find(j => (j as ISortable).SortNumber == (item as ISortable).SortNumber) as ISortable).SortNumber - 1;
                    item.SortNumber = newsort;
                }
            }
        }
        /// <summary>                
        /// 上移                     
        /// </summary>               
        [TestMethod]
        public void MoveUp()
        {
            swap(list, 5, 1);
            list.OrderBy(i => i.SortNumber).ToList().ForEach(i => Console.WriteLine(i.Id + "-" + i.Name + "-" + i.SortNumber));
        }
        /// <summary>
        /// 下移
        /// </summary>
        [TestMethod]
        public void MoveDown()
        {
            swap(list, 5, 7);
            list.OrderBy(i => i.SortNumber).ToList().ForEach(i => Console.WriteLine(i.Id + "-" + i.Name + "-" + i.SortNumber));

        }

    }
}
