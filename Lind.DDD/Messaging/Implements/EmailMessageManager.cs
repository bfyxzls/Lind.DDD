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
    public class EmailMessageManager : IMessageManager
    {
        #region Singleton
        private static object lockObj = new object();
        public static EmailMessageManager Instance;

        static string email_Address = ConfigManager.Config.Messaging.Email_Address;
        static string email_DisplayName = ConfigManager.Config.Messaging.Email_DisplayName;
        static string email_Host = ConfigManager.Config.Messaging.Email_Host;
        static string email_Password = ConfigManager.Config.Messaging.Email_Password;
        static int email_Port = ConfigManager.Config.Messaging.Email_Port;
        static string email_UserName = ConfigManager.Config.Messaging.Email_UserName;

        static EmailMessageManager()
        {
            lock (lockObj)
            {
                if (Instance == null)
                    Instance = new EmailMessageManager();
            }
        }
        private EmailMessageManager()
        { }
        #endregion

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
            try
            {
                if (recipients != null && recipients.Any())
                {
                    using (SmtpClient client = new SmtpClient()
                    {
                        Host = email_Host,
                        Port = email_Port,
                        Credentials = new NetworkCredential(email_UserName, email_Password),
                        EnableSsl = false,//设置为true会出现"服务器不支持安全连接的错误"
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                    })
                    {

                        #region Send Message
                        var mail = new MailMessage
                        {
                            From = new MailAddress(email_Address, email_DisplayName),
                            Subject = subject,
                            Body = body,
                            IsBodyHtml = true,
                        };
                        MailAddressCollection mailAddressCollection = new MailAddressCollection();
                        recipients.ToList().ForEach(i =>
                        {
                            //email有效性验证
                            if (new Regex(@"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$").IsMatch(i))
                                mail.To.Add(i);
                        });

                        client.SendCompleted += new SendCompletedEventHandler(client_SendCompleted);
                        client.SendAsync(mail, recipients);
                        #endregion

                    }
                }
                return 1;
            }
            catch (Exception ex)
            {
                LoggerFactory.Instance.Logger_Info(ex.Message);
                return 0;
            }
        }
    }
}
