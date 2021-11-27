using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Test_Service;
using RabbitMQ_Test_Service.Models;
using System;
using System.IO;
using System.Text;

namespace RabbitMQTestProgram.Handlers
{
    public class MQReceiver
    {
        private readonly ConnectionFactory Factory;
        private readonly FileService FileService;
        private readonly string QueueName;

        public MQReceiver(IConfiguration configuration)
        {
            Factory = new ConnectionFactory() { HostName = configuration["RabbitQueue"] };
            FileService = new FileService(configuration);
            QueueName = configuration["QueueName"];
        }

        public void Receive()
        {
            using (IConnection connection = Factory.CreateConnection())
            {
                using IModel channel = connection.CreateModel();

                channel.QueueDeclare(queue: QueueName, durable: true, exclusive: false, autoDelete: false, arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    FileService.WriteToIncomming(body);
                };

                channel.BasicConsume(QueueName, true, consumer);
            }
        }
    }
}