using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace Lind.DDD.Commons
{
    /// <summary>
    /// 通用消息序列类
    /// </summary>
    public class ReturnMessage : IEnumerable<string>//支持迭代
    {
        /// <summary>
        /// 是否完成
        /// </summary>
        public bool IsComplete
        {
            get
            {
                return messageList.Count == 0;//如果没有异常记录，返回返回为true
            }
        }

        /// <summary>
        /// 可以返回一个IEntity类型的实体
        /// </summary>
        public object Entity { get; set; }

        /// <summary>
        /// 消息序列
        /// </summary>
        List<string> messageList = new List<string>();

        /// <summary>
        /// 向序列中添加新的项
        /// </summary>
        /// <param name="item"></param>
        public void AddItem(string item)
        {
            this.messageList.Add(item);
        }

        /// <summary>
        /// 向序列中追加一个新的序列
        /// </summary>
        /// <param name="itemRange">序列对象</param>
        public void AddItemRange(IEnumerable<string> itemRange)
        {
            this.messageList.AddRange(itemRange);
        }
        /// <summary>
        /// 消息对象
        /// </summary>
        public Object Object { get; set; }

        /// <summary>
        /// 清空所有现有项
        /// </summary>
        public void Clear()
        {
            this.messageList.Clear();
        }

        /// <summary>
        /// 索引器
        /// </summary>
        /// <param name="index">索引</param>
        /// <returns>相应值</returns>
        public string this[int index]
        {
            get { return this.messageList[index]; }

        }

        /// <summary>
        /// 得到所有消息，每条用逗号分开
        /// </summary>
        /// <returns></returns>
        public string GetMessage()
        {
            return string.Join(",", messageList);
        }

        /// <summary>
        /// 得到返回的第一个消息
        /// </summary>
        /// <returns></returns>
        public string GetFirstMessage()
        {
            return messageList.FirstOrDefault();
        }

        #region IEnumerable<string> 成员

        public IEnumerator<string> GetEnumerator()
        {
            return this.messageList.GetEnumerator();
        }

        #endregion
        　
        #region IEnumerable 成员

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.messageList.GetEnumerator();
        }

        #endregion
    }
}
