using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQTestProgram.Handlers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Test_Service
{
    public class RabbitPublishWorker : BackgroundService
    {
        private readonly ILogger<RabbitPublishWorker> _logger;
        private string rabbitHost;
        private MQPublisher Publisher;
        private readonly string QueueName = "SimpleMessage";

        public RabbitPublishWorker(ILogger<RabbitPublishWorker> logger)
        {
            _logger = logger;
            rabbitHost = ConfigurationManager.AppSettings["RabbitQueue"];
            rabbitHost ??= "localhost";
            Publisher = new MQPublisher(rabbitHost);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                try
                {
                    Publisher.Publish(QueueName);
                }
                catch (Exception e)
                {
                    _logger.LogError("No can has read");
                }

                await Task.Delay(1000, stoppingToken);
            }
        }
    }
}
