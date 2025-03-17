using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;

namespace BusinessLayer.Service
{
    public class RabbitMQConsumerService : IRabbitMQConsumerService
    {
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public RabbitMQConsumerService(IConfiguration configuration)
        {
            _hostname = configuration["RabbitMQ:HostName"];
            _queueName = configuration["RabbitMQ:QueueName"];
            _username = configuration["RabbitMQ:UserName"];
            _password = configuration["RabbitMQ:Password"];
        }

        public void ConsumeMessages()
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"📩 Message Received: {message}");
            };

            channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

            Console.WriteLine("🚀 RabbitMQ Consumer is running...");
        }
    }
}
