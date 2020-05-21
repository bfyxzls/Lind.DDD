using Lind.DDD.FastSocket.Client;
using Lind.DDD.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.LindMQ
{
    /// <summary>
    /// 生产者
    /// </summary>
    public class ProducerManager
    {
        ProducerSetting _producerSetting;
        AsyncBinarySocketClient _client;
        public ProducerManager(ProducerSetting producerSetting)
        {
            _producerSetting = producerSetting;
            _client = new AsyncBinarySocketClient(8192, 8192, _producerSetting.Timeout, _producerSetting.Timeout);
            _client.RegisterServerNode(_producerSetting.BrokerName, new IPEndPoint(IPAddress.Parse(_producerSetting.BrokerAddress), _producerSetting.BrokerPort));

        }
        /// <summary>
        /// 推入消息
        /// </summary>
        /// <param name="body"></param>
        public PushResult Push(MessageBody body)
        {
            var result = new PushResult { PushStatus = PushStatus.Succeed };
            try
            {
                _client.Send(
                             "LindQueue_Push",
                             SerializeMemoryHelper.SerializeToBinary(body),
                             res => res.Buffer).ContinueWith(c =>
                             {
                                 if (c.IsFaulted)
                                 {
                                     throw c.Exception;
                                 }
                                 Console.WriteLine(SerializeMemoryHelper.DeserializeFromBinary(c.Result));
                                 Logger.LoggerFactory.Instance.Logger_Debug("LindQueue_Push发送结果：" + Encoding.UTF8.GetString(c.Result));
                             }).Wait();
            }
            catch (Exception ex)
            {
                result.PushStatus = PushStatus.Failed;
                result.ErrorMessage = ex.Message;
            }
            return result;

        }



    }
}
