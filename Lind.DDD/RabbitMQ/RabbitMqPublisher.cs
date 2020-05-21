using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lind.DDD.RabbitMQ
{
    /// <summary>
    /// RabbitMq消息生产者
    /// </summary>
    public class RabbitMqPublisher
    {
        private string _uri;
        private readonly string exchangeName = "";
        private readonly IConnection connection;
        private readonly IModel channel;
        private static object lockObj = new object();
        /// <summary>
        /// 初始化
        /// 子类去实现相关的rabbit地址,端口和授权
        /// </summary>
        /// <param name="uri">消息服务器地址</param>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="exchangeName">交换器,为空表示分发模式,否则为广播模式</param>
        public RabbitMqPublisher(string uri = "amqp://localhost:5672", string userName = "", string password = "", string exchangeName = "")
        {

            _uri = uri;
            var factory = new ConnectionFactory()
            {
                Uri = _uri
            };
            if (!string.IsNullOrWhiteSpace(exchangeName))
                this.exchangeName = exchangeName;
            if (!string.IsNullOrWhiteSpace(userName))
                factory.UserName = userName;
            if (!string.IsNullOrWhiteSpace(userName))
                factory.Password = password;
            connection = factory.CreateConnection();
            this.channel = connection.CreateModel();
        }

        /// <summary>
        /// 将消息推送到服务器
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="queue"></param>
        /// <param name="message"></param>
        public void Publish<TMessage>(string queue, TMessage message)
        {
            channel.QueueDeclare(queue: queue,//队列名
                                     durable: false,//是否持久化
                                     exclusive: false,//排它性
                                     autoDelete: false,//一旦客户端连接断开则自动删除queue
                                     arguments: null);//如果安装了队列优先级插件则可以设置优先级

            var json = Utils.SerializeMemoryHelper.SerializeToJson(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            Console.WriteLine("向服务器{0}推消息", _uri);
            channel.BasicPublish(exchange: this.exchangeName, routingKey: queue, basicProperties: null, body: bytes);
        }

        /// <summary>
        /// 广播消息,需要在初始化时为exchangeName赋值
        /// </summary>
        /// <typeparam name="TMessage"></typeparam>
        /// <param name="message"></param>
        public void Publish<TMessage>(TMessage message)
        {
            const string ROUTING_KEY = "";
            channel.ExchangeDeclare(this.exchangeName, "fanout");//广播
            var json = Utils.SerializeMemoryHelper.SerializeToJson(message);
            var bytes = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(this.exchangeName, ROUTING_KEY, null, bytes);//不需要指定routing key，设置了fanout,指了也没有用.
            Console.WriteLine(DateTime.Now + " 向服务器{0}推消息", _uri);
        }


    }
}
