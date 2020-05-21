using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text.RegularExpressions;
using Lind.DDD.Logger;
using System.Configuration;
using Lind.DDD.ConfigConstants;
namespace Lind.DDD.Messaging.Implements
{
    /// <summary>
    ///Email消息服务
    /// </summary>
    public class EmptyMessageManager : IMessageManager
    {
        public EmptyMessageManager() { }


        #region IMessageManager 成员

        void client_SendCompleted(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
        {
            string arr = null;
            (e.UserState as List<string>).ToList().ForEach(i => { arr += i; });
            //发送完成后要做的事件，可能是写日志
        }

        #endregion

        public int Send(string recipient, string subject, string body)
        {
            return Send(recipient, subject, body, null, null);
        }

        public int Send(string recipient, string subject, string body, Action errorAction = null, Action successAction = null)
        {
            return Send(new List<string>() { recipient }, subject, body, successAction, errorAction);
        }

        public int Send(IEnumerable<string> recipients, string subject, string body, Action errorAction = null, Action successAction = null)
        {
            Console.WriteLine("recipients:{0},subject:{1}", string.Join(",", recipients), subject);
            return 0;
        }
    }
}
