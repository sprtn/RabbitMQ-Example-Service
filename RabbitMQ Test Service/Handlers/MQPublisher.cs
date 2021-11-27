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

        private readonly int MaxRetries;
        private readonly int FilesToCreate;
        private int cnt;

        public MQPublisher(IConfigurationRoot configuration)
        {
            Factory = new ConnectionFactory() { HostName = configuration["RabbitQueue"] };
            FileService = new FileService(configuration);
            if (!int.TryParse(configuration["FilesToCreate"], out FilesToCreate))
                FilesToCreate = 100;
            if (!int.TryParse(configuration["MaxRetries"], out MaxRetries))
                MaxRetries = 1;
        }

        internal void Publish(string queue, SimpleMessage outgoingMsg = null, int retryNo = 0)
        {
            // Only create so many dummy files
            if (outgoingMsg == null && cnt > FilesToCreate) return;
            // Alter body to track counter
            if (outgoingMsg != null) outgoingMsg.Body = $"Even morem ipsum sin dolor amet:  {cnt++}";
            // Dont get hard loops of messages
            if (retryNo > MaxRetries) FileService.SaveToError(outgoingMsg);

            try
            {
                outgoingMsg ??= CreateSimpleMessageExample();
                using (var connection = Factory.CreateConnection())
                {
                    using var channel = connection.CreateModel();
                    var body = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(outgoingMsg));
                    channel.BasicPublish("", queue, null, body);
                };
            }
            catch (Exception e)
            {
                Console.WriteLine($"Could not publish to MQ: {e.Message}");
                Publish(queue, outgoingMsg, retryNo++);
            }
        }

        private SimpleMessage CreateSimpleMessageExample()
        {
            return new SimpleMessage()
            {
                Body = $"Lorem ipsum sin dolor amet: {cnt++}",
                Sender = "Vegard Pihl Bratteng"
            };
        }
    }
}