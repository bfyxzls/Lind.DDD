using Lind.DDD.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.Utils
{
    /// <summary>
    /// 算法帮助类
    /// 作者：仓储大叔
    /// </summary>
    public class AlgorithmsHelper
    {
        /// <summary>
        /// 排序，上移和下移
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="id"></param>
        /// <param name="newSort"></param>
        public static void Sortable_Up_Down<T>(List<T> list, int id, int newSort) where T : class
        {

            var old = list.Find(i => (i as Entity).Id == id);
            if (old == null || (old as ISortBehavor).SortNumber == newSort)
                return;


            if ((old as ISortBehavor).SortNumber > newSort)
            {
                (old as ISortBehavor).SortNumber = newSort;

                foreach (ISortBehavor item in list.FindAll(i => (i as Entity).Id != id && (i as ISortBehavor).SortNumber >= newSort))
                {

                    item.SortNumber += 1;
                }

            }
            else
            {
                (old as ISortBehavor).SortNumber = newSort;


                foreach (ISortBehavor item in list.FindAll(i => (i as Entity).Id != id && (i as ISortBehavor).SortNumber <= newSort))
                {

                    item.SortNumber -= 1;
                }
            }
            list.OrderBy(i => (i as ISortBehavor).SortNumber)
                .ToList()
                .ForEach(i =>
            {
                Console.WriteLine((i as Entity).Id + "sort:" + (i as ISortBehavor).SortNumber);
            }
            );
        }
    }
}
