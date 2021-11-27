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

        public MQReceiver(IConfiguration configuration)
        {
            Factory = new ConnectionFactory() { HostName = configuration["RabbitQueue"] };
            FileService = new FileService(configuration);
        }

        public void Receive(string rabbitQueue)
        {
            using IConnection connection = Factory.CreateConnection();
            using IModel channel = connection.CreateModel();

            channel.QueueDeclare(queue: rabbitQueue,
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            if (channel.IsClosed) 
            {
                Console.WriteLine("WARNING: Unable to contact host queue");
            };

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                FileService.WriteToIncomming(body);
            };

            channel.BasicConsume(rabbitQueue, true, consumer);
        }
    }
}