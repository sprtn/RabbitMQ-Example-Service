using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ_Test_Service;
using RabbitMQ_Test_Service.Models;
using System;
using System.Text;


namespace RabbitMQTestProgram.Handlers
{
    public class MQPublisher
    {
        ConnectionFactory factory;
        string rabbitQueue;
        int cnt = 0;
        FileService FileService;

        public MQPublisher(string hostName)
        {
            rabbitQueue = hostName;
            factory = new ConnectionFactory() { HostName = rabbitQueue };
            FileService = new FileService("F:\\RabbitMQ\\test\\");
        }

        internal void Publish(string queueName, SimpleMessage msg = null, int retries = 0)
        {
            if (msg == null && cnt > 100)
                return;
            
            if (msg != null)
                msg.Body = $"this is another test {cnt++}";

            if (retries > 2)
                FileService.LocalStorage(msg);

            try
            {
                msg ??= CreateRandomMessage();
                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                        channel.BasicPublish(
                            exchange: "",
                            routingKey: queueName,
                            basicProperties: null,
                            body: body);
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not publish to MQ: {e.Message}");
                Publish(queueName, msg, retries++);
            }
        }

        private SimpleMessage CreateRandomMessage()
        {
            return new SimpleMessage()
            {
                Header = "Simple Message Format",
                Body = $"this is a test {cnt++}",
                Sender = "Vegard"
            };
        }
    }
}