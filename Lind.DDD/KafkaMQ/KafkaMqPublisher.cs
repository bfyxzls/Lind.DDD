using KafkaNet;
using KafkaNet.Model;
using KafkaNet.Protocol;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lind.DDD.KafkaMQ
{
    /// <summary>
    /// Kafka发布者
    /// </summary>
    public class KafkaMqPublisher
    {
        /// <summary>
        /// 生产者
        /// </summary>
        Producer _client;
        string _topic;
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="broker"></param>
        private KafkaMqPublisher(string broker = "http://192.168.128.128:9092/", string topic = "test")
        {
            var options = new KafkaOptions(new Uri(broker));
            var router = new BrokerRouter(options);
            _client = new Producer(router);
            _topic = topic;
        }

        public void Publish<TMessage>(string topic, TMessage message)
        {
            var currentDatetime = DateTime.Now;
            var key = currentDatetime.Millisecond.ToString();
            var events = new[] { new Message(Utils.SerializeMemoryHelper.SerializeToJson(message), key) };
            _client.SendMessageAsync(topic, events).Wait(1);

            Console.WriteLine("Produced: Key: {0}. Message: {1}", key, Encoding.UTF8.GetString(events[0].Value));
        }

        public void Publish<TMessage>(TMessage message)
        {
            Publish<TMessage>(_topic, message);
        }
    }
}
