using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ_Test_Service.Models;
using System;
using System.IO;
using System.Text;

namespace RabbitMQTestProgram.Handlers
{
    public class MQReceiver
    {
        ConnectionFactory factory;
        private string rabbitHost;

        public MQReceiver(string hostName)
        {
            rabbitHost = hostName;
            factory = new ConnectionFactory() { HostName = rabbitHost };
        }

        public void Receive(string rabbitQueue)
        {
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: rabbitQueue,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        SimpleMessage msg = Newtonsoft.Json.JsonConvert.DeserializeObject<SimpleMessage>(Encoding.UTF8.GetString(body));
                        Console.WriteLine(" [x] Received {0}", msg);

                        File.WriteAllBytes("F:\\RabbitMQ\\test\\incomming\\Received-" + DateTime.Now.Ticks.ToString() + ".simpjson", body);
                    };

                    channel.BasicConsume(queue: rabbitQueue,
                                         autoAck: true,
                                         consumer: consumer);
                }
            }            
        }
    }
}