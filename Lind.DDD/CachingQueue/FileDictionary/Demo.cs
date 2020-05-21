using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.CachingQueue.FileDictionary
{
    public class Demo
    {
        #region Delegates & Events
        /// <summary>
        /// 当客户端向服务端发完数据后，触发
        /// </summary>
        protected static event Action<SendingResult> SendingData;
        /// <summary>
        /// 触发SendData事件
        /// </summary>
        /// <param name="obj"></param>
        protected static void OnSendingData(SendingResult obj)
        {
            if (SendingData != null)
            {
                SendingData(obj);
            }
        }
        /// <summary>
        /// 当客户端发送数据到服务端，并成功返回正确的结果后，触发
        /// </summary>
        protected static event Action<SentResult> SentData;
        /// <summary>
        /// 触发OnSendDataSuccess事件
        /// </summary>
        /// <param name="obj"></param>
        protected static void OnSentData(SentResult obj)
        {
            if (SentData != null)
            {
                SentData(obj);
            }
        }
        #endregion

        static Demo()
        {
            SentData += (obj) =>
            {
                Console.WriteLine(".........tableName成功写入服务端缓存队列..........");
                FileDictionaryManager<SentResult>.Instance.Add(obj.Db_TableName, obj);
            };
            SendingData += (obj) =>
            {
                Console.WriteLine(".........客户端向服务端发完数据后写自己的日志..........");
                FileDictionaryManager<SendingResult>.Instance.Add(obj.BatchNumber, obj);
            };
        }
        public static void DataSend()
        {


            //发送前
            OnSendingData(new SendingResult { });

            //发送后
            OnSentData(new SentResult { });
        }
    }
}
