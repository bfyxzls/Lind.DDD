using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading;
using System.Threading.Tasks;

namespace Lind.DDD.UnitTest
{
    [TestClass]
    public class ReaderWriterLockTest
    {
        private static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();
        static string TestStr = "good";


        /// <summary>
        /// 共享锁和互斥锁
        /// </summary>
        void TestReadWrite(string ress)
        {
            rwLock.EnterReadLock();
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "线程{0}读到了数据", Thread.CurrentThread.ManagedThreadId);
            Thread.Sleep(10000);
            rwLock.ExitReadLock();

            rwLock.EnterWriteLock();
            string res = ress;
            Console.WriteLine(DateTime.Now.ToLongTimeString() + "线程{0}写了数据{1}", Thread.CurrentThread.ManagedThreadId, res);
            Thread.Sleep(10000);
            rwLock.ExitWriteLock();
        }




        [TestMethod]
        public void ReadWriteLock()
        {
            //多线程的并行
            Parallel.Invoke(() =>
            {
                TestReadWrite("1");
            }, () =>
            {
                TestReadWrite("2");
            }, () =>
            {
                TestReadWrite("3");
            });
        }
    }
}
