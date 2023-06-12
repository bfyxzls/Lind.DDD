using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Lind.DDD.RabbitMQ
{
    /// <summary>
    /// RabbitMq消息消费者
    /// </summary>
    public class RabbitMqSubscriber : IDisposable
    {
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RabbitMqSubscriber(string uri = "amqp://localhost:5672", string userName = "", string password = "", string exchangeName = "", string queue = "")
        {
            var factory = new ConnectionFactory { Uri = uri, UserName = userName, Password = password };
            _exchangeName = exchangeName;
            _queueName = string.IsNullOrWhiteSpace(queue) ? "default" : queue;
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            if (string.IsNullOrWhiteSpace(_exchangeName))
            {
                _channel.QueueDeclare(
                    queue: _queueName,
                    durable: false,
                    exclusive: false,
                    autoDelete: false,
                    arguments: null);
            }
            else
            {
                _channel.ExchangeDeclare(_exchangeName, "fanout");
                var queueOk = _channel.QueueDeclare();
                _queueName = queueOk.QueueName;
                _channel.QueueBind(_queueName, _exchangeName, string.Empty);
            }
        }

        public void Subscribe<TMessage>(Action<TMessage> callback)
        {
            var consumer = new EventingBasicConsumer(_channel);
            consumer.Received += (sender, e) =>
            {
                var body = e.Body;
                var json = Encoding.UTF8.GetString(body);
                callback(Utils.SerializeMemoryHelper.DeserializeFromJson<TMessage>(json));
                _channel.BasicAck(e.DeliveryTag, multiple: false);
            };
            _channel.BasicConsume(queue: _queueName, noAck: false, consumer: consumer);
            Console.WriteLine(" [*] Waiting for messages. To exit press CTRL+C");
        }

        public void Subscribe<TMessage>(string topic, Action<TMessage> callback) where TMessage : class, new()
        {
            Subscribe(callback);
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
