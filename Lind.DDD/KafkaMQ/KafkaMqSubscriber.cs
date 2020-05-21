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
    /// 消息的消费者,有自己的消费进度管理
    /// </summary>
    public class KafkaMqSubscriber
    {
        /// <summary>
        /// 消费者
        /// </summary>
        Consumer _consumer;
        string _topic;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="broker">消息服务器地址</param>
        /// <param name="topic">主题</param>
        public KafkaMqSubscriber(string brokerUri = "http://192.168.128.128:9092/", string topic = "test")
        {
            var options = new KafkaOptions(new Uri(brokerUri));
            var router = new BrokerRouter(options);
            _consumer = new Consumer(new ConsumerOptions(topic, router));
            _topic = topic;
        }

        /// <summary>
        /// 消费
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="callback"></param>
        public void Subscribe<TMessage>(Action<TMessage> callback) where TMessage : class, new()
        {
            _consumer.SetOffsetPosition(new OffsetPosition(0, 260));//设置要消费的偏移量，即从哪里开始消费，避免重复消费
            foreach (var message in _consumer.Consume())
            {
                Console.WriteLine("Response: Partition {0},Offset {1}", message.Meta.PartitionId, message.Meta.Offset);
                var value = Encoding.UTF8.GetString(message.Value);
                callback(Utils.SerializeMemoryHelper.DeserializeFromJson<TMessage>(value));
                //处理成功后把偏移量存储下来
                //MessageMetadata.Offset
            }
        }

        /// <summary>
        /// 消费
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="topic"></param>
        /// <param name="callback"></param>
        public void Subscribe<TMessage>(string topic, Action<TMessage> callback) where TMessage : class, new()
        {
            Subscribe<TMessage>(_topic, callback);
        }
    }
}

