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

namespace Lind.DDD.CachingQueue
{
    /// <summary>
    /// 结果集类型
    /// </summary>
    public enum ResultType
    {
        NoDo = 0,
        Success = 1,
        Fail = 2,
    }
    /// <summary>
    /// 数据同步结果对象
    /// </summary>
    [XmlRoot("AsyncResult")]
    public class AsyncResult
    {
        /// <summary>
        /// 主键＝库名+表名
        /// </summary>
        public string PrimaryKey { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string DbName { get; set; }
        /// <summary>
        /// 数据表名称
        /// </summary>
        public string TableName { get; set; }
        /// <summary>
        /// 批次开始主键号
        /// </summary>
        public long StartId { get; set; }
        /// <summary>
        /// 批次结束主键号
        /// </summary>
        public long EndId { get; set; }

    }



    /// <summary>
    /// 结果泛型字典
    /// </summary>
    public class ResultDictionary<T> :
        IDictionary<string, T> where T : class
    {


        #region Constructors & Fields
        static ResultDictionary()
        {
            _instance = new ResultDictionary<T>();

            if (!Directory.Exists(_folder))
                Directory.CreateDirectory(_folder);

            if (!File.Exists(_fileName))
                _dic = new SerializableDictionary<string, T>();
            else
                _dic = SerializationHelper.DeserializeFromXml<SerializableDictionary<string, T>>(_fileName) ?? new SerializableDictionary<string, T>();

            sysTimer.Enabled = true;
            sysTimer.AutoReset = true;
            sysTimer.Elapsed += sysTimer_Elapsed;
            sysTimer.Start();
        }

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

        ~ResultDictionary()
        {
        }

        /// <summary>
        /// 字典结果集，对外不公开，只开放指定方法
        /// </summary>
        private readonly static SerializableDictionary<string, T> _dic;
        /// <summary>
        /// 结果集文件夹
        /// </summary>
        private static string _folder = Environment.CurrentDirectory + "\\" + (System.Configuration.ConfigurationManager.AppSettings["ResultQueuePath"] ?? "ResultQueue");
        /// <summary>
        /// 结果集文件名   
        /// </summary>
        private static string _fileName = _folder + "\\ResultQueue.xml";
        /// <summary>
        /// 锁对象
        /// </summary>
        private static object lockObj = new object();
        /// <summary>
        /// 轮训时间
        /// </summary>
        private static Timer sysTimer = new Timer(1000);
        /// <summary>
        /// 表字典
        /// </summary>
        private static ResultDictionary<T> _instance;
        #endregion

        #region Singleton
        public static ResultDictionary<T> Instance
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
            if (_dic.ContainsKey(key))//清除老进度，写入新进度
                _dic.Remove(key);

            _dic.Add(key, value);

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
            return _dic.Remove(key);
        }

        public bool TryGetValue(string key, out T value)
        {
            throw new NotImplementedException();
        }

        public ICollection<T> Values
        {
            get { return _dic.Values; }
        }

        public T this[string key]
        {
            get
            {
                if (_dic.ContainsKey(key))
                    return _dic[key];
                else
                    return null;
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

    /// <summary>  
    /// 支持XML序列化的泛型 Dictionary  
    /// </summary>  
    /// <typeparam name="TKey"></typeparam>  
    /// <typeparam name="TValue"></typeparam>  
    [XmlRoot("SerializableDictionary")]
    public class SerializableDictionary<TKey, TValue> :
        Dictionary<TKey, TValue>,
        IXmlSerializable
    {

        #region 构造函数
        public SerializableDictionary()
            : base()
        {
        }
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary)
            : base(dictionary)
        {
        }

        public SerializableDictionary(IEqualityComparer<TKey> comparer)
            : base(comparer)
        {
        }

        public SerializableDictionary(int capacity)
            : base(capacity)
        {
        }
        public SerializableDictionary(int capacity, IEqualityComparer<TKey> comparer)
            : base(capacity, comparer)
        {
        }
        protected SerializableDictionary(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
        #endregion
        #region IXmlSerializable Members
        public System.Xml.Schema.XmlSchema GetSchema()
        {
            return null;
        }
        /// <summary>  
        /// 从对象的 XML 表示形式生成该对象  
        /// </summary>  
        /// <param name="reader"></param>  
        public void ReadXml(System.Xml.XmlReader reader)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            bool wasEmpty = reader.IsEmptyElement;
            reader.Read();
            if (wasEmpty)
                return;
            while (reader.NodeType != System.Xml.XmlNodeType.EndElement)
            {
                reader.ReadStartElement("item");
                reader.ReadStartElement("key");
                TKey key = (TKey)keySerializer.Deserialize(reader);
                reader.ReadEndElement();
                reader.ReadStartElement("value");
                TValue value = (TValue)valueSerializer.Deserialize(reader);
                reader.ReadEndElement();
                this.Add(key, value);
                reader.ReadEndElement();
                reader.MoveToContent();
            }
            reader.ReadEndElement();
        }

        /// <summary>  
        /// 将对象转换为其 XML 表示形式  
        /// </summary>  
        /// <param name="writer"></param>  
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            XmlSerializer keySerializer = new XmlSerializer(typeof(TKey));
            XmlSerializer valueSerializer = new XmlSerializer(typeof(TValue));
            foreach (TKey key in this.Keys)
            {
                writer.WriteStartElement("item");
                writer.WriteStartElement("key");
                keySerializer.Serialize(writer, key);
                writer.WriteEndElement();
                writer.WriteStartElement("value");
                TValue value = this[key];
                valueSerializer.Serialize(writer, value);
                writer.WriteEndElement();
                writer.WriteEndElement();
            }
        }
        #endregion
    }
}
