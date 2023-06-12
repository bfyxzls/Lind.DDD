using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lind.DDD.RabbitMQ
{
    public class RabbitMqPublisher : IDisposable
    {
        private readonly string _uri;
        private readonly string _exchangeName;
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private static readonly object _lockObj = new object();

        public RabbitMqPublisher(string uri = "amqp://localhost:5672", string userName = "", string password = "", string exchangeName = "")
        {
            _uri = uri;
            _exchangeName = exchangeName;

            var factory = new ConnectionFactory { Uri = _uri, UserName = userName, Password = password };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            if (!string.IsNullOrWhiteSpace(_exchangeName))
                _channel.ExchangeDeclare(_exchangeName, "fanout");
        }

        public void Publish<TMessage>(string queue, TMessage message)
        {
            lock (_lockObj)
            {
                DeclareQueue(queue);
                var json = Utils.SerializeMemoryHelper.SerializeToJson(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(_exchangeName, queue, null, bytes);
                Console.WriteLine("向服务器{0}推消息", _uri);
            }
        }

        public void Publish<TMessage>(TMessage message)
        {
            lock (_lockObj)
            {
                const string ROUTING_KEY = "";
                var json = Utils.SerializeMemoryHelper.SerializeToJson(message);
                var bytes = Encoding.UTF8.GetBytes(json);
                _channel.BasicPublish(_exchangeName, ROUTING_KEY, null, bytes);
                Console.WriteLine(DateTime.Now + " 向服务器{0}推消息", _uri);
            }
        }

        private void DeclareQueue(string queue)
        {
            _channel.QueueDeclare(queue, durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();
            _connection?.Close();
            _connection?.Dispose();
        }
    }

}
