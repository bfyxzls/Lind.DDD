using Lind.DDD.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Serialization;

namespace Lind.DDD.CachingQueue.FileDictionary
{
    /// <summary>
    /// 文件泛型字典
    /// </summary>
    public class FileDictionaryManager<T> :
        IDictionary<string, T> where T : class
    {

        #region Constructors & Destructor & Fields
        static FileDictionaryManager()
        {
            _instance = new FileDictionaryManager<T>();

            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            if (!File.Exists(_fileName))
                _dic = new SerializableDictionary<string, T>();
            else
                _dic = SerializationHelper.DeserializeFromXml<SerializableDictionary<string, T>>(_fileName)
                    ?? new SerializableDictionary<string, T>();

            sysTimer = new Timer(5000);
            sysTimer.Enabled = true;
            sysTimer.AutoReset = true;
            sysTimer.Elapsed += sysTimer_Elapsed;
            sysTimer.Start();
        }

        ~FileDictionaryManager() { }

        static void sysTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            #region 写日志文件
            //当对象被释放时再进行I/O操作
            lock (lockObj)
            {
                SerializationHelper.SerializeToXml(_fileName, _dic);
            }
            #endregion
        }

        /// <summary>
        /// 字典结果集，对外不公开，只开放指定方法
        /// </summary>
        private readonly static SerializableDictionary<string, T> _dic;
        /// <summary>
        /// 结果集文件夹
        /// </summary>
        private static string _folder = Environment.CurrentDirectory
            + "\\"
            + (System.Configuration.ConfigurationManager.AppSettings["ResultQueuePath"] ?? "ResultQueue");
        /// <summary>
        /// 结果集文件名,在派生类中可以去重新定义它
        /// </summary>
        private static string _fileName = _folder
            + "\\"
            + typeof(T).Name + ".xml";
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object lockObj = new object();
        /// <summary>
        /// 轮训时间
        /// </summary>
        private static Timer sysTimer;
        /// <summary>
        /// 表字典
        /// </summary>
        private static FileDictionaryManager<T> _instance;
        #endregion

        #region Singleton Instance
        public static FileDictionaryManager<T> Instance
        {
            get
            {
                return _instance;
            }
        }
        #endregion

        #region IDictionary<string,ResultType> 成员

        public void Add(string key, T value)
        {
            _dic.TryAdd(key, value);
        }

        public bool ContainsKey(string key)
        {
            return _dic.ContainsKey(key);
        }

        public ICollection<string> Keys
        {
            get { return _dic.Keys; }
        }

        public bool Remove(string key)
        {
            T val;
            return _dic.TryRemove(key, out val);
        }

        public bool TryGetValue(string key, out T value)
        {
            return _dic.TryGetValue(key, out value);
        }

        public ICollection<T> Values
        {
            get { return _dic.Values; }
        }

        public T this[string key]
        {
            get
            {
                return _dic[key];
            }
            set
            {
                _dic[key] = value;
            }
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,ResultType>> 成员

        public IEnumerator<KeyValuePair<string, T>> GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        #endregion

        #region IEnumerable 成员

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _dic.GetEnumerator();
        }

        #endregion

        #region ICollection<KeyValuePair<string,T>> 成员

        public void Add(KeyValuePair<string, T> item)
        {
            throw new NotImplementedException();
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<string, T> item)
        {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<string, T>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public int Count
        {
            get { throw new NotImplementedException(); }
        }

        public bool IsReadOnly
        {
            get { throw new NotImplementedException(); }
        }

        public bool Remove(KeyValuePair<string, T> item)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
