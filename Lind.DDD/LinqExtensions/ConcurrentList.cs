using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace System.Collections.Generic
{
    /// <summary>
    /// 线程安全的List集合
    /// </summary>
    /// <typeparam name="T">类型</typeparam>
    public class ConcurrentList<T> : IList<T>
    {
    
        /// <summary>
        ///创建一个纯种安全的List集合对像
        /// </summary>
        public ConcurrentList()
        {
            list = new List<T>();
        }

        /// <summary>
        /// 内部的索引器访问对像
        /// </summary>
        protected List<T> list;

        /// <summary>
        /// 锁对像
        /// </summary>
        protected int lockInt;

        /// <summary>
        /// 提供一个互斥锁
        /// </summary>
        System.Threading.SpinLock splock = new System.Threading.SpinLock();

        #region IList
        /// <summary>
        /// 搜索指定的对象，并返回整个 System.Collections.Generic.List<T> 中第一个匹配项的从零开始的索引。
        /// </summary>
        /// <param name="item">对像</param>
        /// <returns></returns>
        public int IndexOf(T item)
        {

            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                return list.IndexOf(item);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }

        }
        /// <summary>
        ///  将元素插入 System.Collections.Generic.List<T> 的指定索引处。
        /// </summary>
        /// <param name="index">指定索引</param>
        /// <param name="item">元素</param>
        public void Insert(int index, T item)
        {

            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                list.Insert(index, item);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }
        /// <summary>
        /// 移除 System.Collections.Generic.List<T> 的指定索引处的元素。
        /// </summary>
        /// <param name="index">指定索引</param>
        public void RemoveAt(int index)
        {

            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                list.RemoveAt(index);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }

        public T this[int index]
        {
            get
            {
                System.Threading.Interlocked.Increment(ref lockInt);
                try
                {
                    return list[index];
                }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref lockInt);
                }
            }
            set
            {
                System.Threading.Interlocked.Increment(ref lockInt);
                try
                {
                    list[index] = value;
                }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref lockInt);
                }
            }
        }

        public void Add(T item)
        {

            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                list.Add(item);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }

        public void Clear()
        {

            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                list.Clear();
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }

        public bool Contains(T item)
        {

            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                return list.Contains(item);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {

            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                list.CopyTo(array, arrayIndex);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }

        public int Count
        {
            get
            {
                System.Threading.Interlocked.Increment(ref lockInt);
                try
                {
                    return list.Count;
                }
                finally
                {
                    System.Threading.Interlocked.Decrement(ref lockInt);
                }
            }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }
        /// <summary>
        ///  从 System.Collections.Generic.List<T> 中移除特定对象的第一个匹配项。
        /// </summary>
        /// <param name="item">移除特定对象</param>
        /// <returns></returns>
        public bool Remove(T item)
        {

            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                return list.Remove(item);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                return list.GetEnumerator();
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        #endregion
        /// <summary>
        ///  创建源 System.Collections.Generic.List<T> 中的元素范围的浅表副本。
        /// </summary>
        /// <param name="index">索引</param>
        /// <param name="count">总数</param>
        /// <returns></returns>
        public List<T> GetRange(int index, int count)
        {
            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                return list.GetRange(index, count);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }
        /// <summary>
        ///  从 System.Collections.Generic.List<T> 中移除一定范围的元素。
        /// </summary>
        /// <param name="index">要移除的元素的范围从零开始的起始索引。</param>
        /// <param name="count">要移除的元素数</param>
        public void RemoveRange(int index, int count)
        {
            bool gotLock = false;
            try
            {
                splock.Enter(ref gotLock);
                list.RemoveRange(index, count);
            }
            finally
            {
                if (gotLock)
                {
                    splock.Exit();
                }
            }
        }
    }

}
