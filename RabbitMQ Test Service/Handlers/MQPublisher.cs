using Microsoft.Extensions.Configuration;
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
        private readonly ConnectionFactory Factory;
        private readonly FileService FileService;
        private int cnt;

        public MQPublisher(IConfigurationRoot configuration)
        {
            Factory = new ConnectionFactory() { HostName = configuration["RabbitQueue"] };
            FileService = new FileService(configuration);
        }

        internal void Publish(string queue, SimpleMessage msg = null, int retries = 0)
        {
            if (msg == null && cnt > 100)
                return;
            
            if (msg != null)
                msg.Body = $"this is another test {cnt++}";

            if (retries > 2)
                FileService.SaveToError(msg);

            try
            {
                msg ??= CreateSimpleMessageExample();
                using (var connection = Factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg));
                        channel.BasicPublish("", queue, null, body);
                    }
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not publish to MQ: {e.Message}");
                Publish(queue, msg, retries++);
            }
        }

        private SimpleMessage CreateSimpleMessageExample()
        {
            return new SimpleMessage()
            {
                Body = $"this is a test {cnt++}",
                Sender = "Vegard"
            };
        }
    }
}