using Microsoft.Extensions.Configuration;
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
        private readonly MQPublisher Publisher;
        private readonly string QueueName = "SimpleMessage";

        public RabbitPublishWorker(ILogger<RabbitPublishWorker> logger)
        {
            _logger = logger;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Publisher = new MQPublisher(configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Minute % 1 == 0 && DateTime.Now.Second == 0 && DateTime.Now.Millisecond < 15)
                    _logger.LogInformation("Publisher running at: {time}", DateTimeOffset.Now);

                try
                {
                    Publisher.Publish(QueueName);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "No can has read");
                }

                await Task.Delay(15, stoppingToken);
            }
        }
    }
}
