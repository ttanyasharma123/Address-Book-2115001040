using System;
using System.Text;
using BusinessLayer.Interface;
using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;

namespace YourNamespace.Services
{
    public class RabbitMQPublisherService : IRabbitMQPublisherService
    {
        private readonly string _hostname;
        private readonly string _queueName;
        private readonly string _username;
        private readonly string _password;

        public RabbitMQPublisherService(IConfiguration configuration)
        {
            _hostname = configuration["RabbitMQ:HostName"];
            _queueName = configuration["RabbitMQ:QueueName"];
            _username = configuration["RabbitMQ:UserName"];
            _password = configuration["RabbitMQ:Password"];
        }

        public void PublishMessage(string message, string routingKey)
        {
            var factory = new ConnectionFactory()
            {
                HostName = _hostname,
                UserName = _username,
                Password = _password
            };

            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: _queueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "", routingKey: _queueName, basicProperties: null, body: body);

            Console.WriteLine($"✅ Message Published: {message}");
        }
    }
}
