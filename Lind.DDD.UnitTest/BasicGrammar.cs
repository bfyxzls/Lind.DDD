using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class BasicGrammar
    {
        [TestMethod]
        public void Hash_Dic()
        {
            Compare(10);
            Compare(10000);
        }

        [TestMethod]
        public void Sort()
        {
            List<int> arr = new List<int>();
          
        }

        static Hashtable _Hashtable;
        static Dictionary<string, object> _Dictionary;
        static void Compare(int dataCount)
        {
            Console.WriteLine("-------------------------------------------------\n");
            _Hashtable = new Hashtable();
            _Dictionary = new Dictionary<string, object>();
            Stopwatch stopWatch = new Stopwatch();
            //HashTable插入dataCount条数据需要时间
            stopWatch.Start();
            for (int i = 0; i < dataCount; i++)
            {
                _Hashtable.Add("Str" + i.ToString(), "Value");
            }
            stopWatch.Stop();
            Console.WriteLine(" HashTable插入" + dataCount + "条数据需要时间：" + stopWatch.Elapsed);

            //Dictionary插入dataCount条数据需要时间
            stopWatch.Reset();
            stopWatch.Start();
            for (int i = 0; i < dataCount; i++)
            {
                _Dictionary.Add("Str" + i.ToString(), "Value");
            }
            stopWatch.Stop();
            Console.WriteLine(" Dictionary插入" + dataCount + "条数据需要时间：" + stopWatch.Elapsed);

            //Dictionary插入dataCount条数据需要时间
            stopWatch.Reset();
            int si = 0;
            stopWatch.Start();
            for (int i = 0; i < _Hashtable.Count; i++)
            {
                si++;
            }
            stopWatch.Stop();
            Console.WriteLine(" HashTable遍历时间：" + stopWatch.Elapsed + " ,遍历采用for方式");

            //Dictionary插入dataCount条数据需要时间
            stopWatch.Reset();
            si = 0;
            stopWatch.Start();
            foreach (var s in _Hashtable)
            {
                si++;
            }
            stopWatch.Stop();
            Console.WriteLine(" HashTable遍历时间：" + stopWatch.Elapsed + " ,遍历采用foreach方式");

            //Dictionary插入dataCount条数据需要时间
            stopWatch.Reset();
            si = 0; 
            stopWatch.Start();
            IDictionaryEnumerator _hashEnum = _Hashtable.GetEnumerator();
            while (_hashEnum.MoveNext())
            {
                si++;
            }
            stopWatch.Stop();
            Console.WriteLine(" HashTable遍历时间：" + stopWatch.Elapsed + " ,遍历采用HashTable.GetEnumerator()方式");

            //Dictionary插入dataCount条数据需要时间
            stopWatch.Reset();
            si = 0;
            stopWatch.Start();
            for (int i = 0; i < _Dictionary.Count; i++)
            {
                si++;
            }
            stopWatch.Stop();
            Console.WriteLine(" Dictionary遍历时间：" + stopWatch.Elapsed + " ,遍历采用for方式");

            //Dictionary插入dataCount条数据需要时间
            stopWatch.Reset();
            si = 0;
            stopWatch.Start();
            foreach (var s in _Dictionary)
            {
                si++;
            }
            stopWatch.Stop();
            Console.WriteLine(" Dictionary遍历时间：" + stopWatch.Elapsed + " ,遍历采用foreach方式");

            //Dictionary插入dataCount条数据需要时间
            stopWatch.Reset();
            si = 0;
            stopWatch.Start();
            _hashEnum = _Dictionary.GetEnumerator();
            while (_hashEnum.MoveNext())
            {
                si++;
            }
            stopWatch.Stop();
            Console.WriteLine(" Dictionary遍历时间：" + stopWatch.Elapsed + " ,遍历采用Dictionary.GetEnumerator()方式");


            Console.WriteLine("\n-------------------------------------------------");
        }

    }
}
