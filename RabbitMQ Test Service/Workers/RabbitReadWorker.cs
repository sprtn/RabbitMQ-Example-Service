using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQTestProgram.Handlers;
using System;
using System.Configuration;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Test_Service
{
    public class RabbitReadWorker : BackgroundService
    {
        private readonly ILogger<RabbitPublishWorker> _logger;
        private readonly MQReceiver Receiver;

        public RabbitReadWorker(ILogger<RabbitPublishWorker> logger)
        {
            _logger = logger;
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();

            Receiver = new MQReceiver(configuration);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                if (DateTime.Now.Minute % 1 == 0 && DateTime.Now.Second == 0 && DateTime.Now.Millisecond < 100)
                    _logger.LogInformation("Reader running at: {time}", DateTimeOffset.Now);

                try
                {
                    Receiver.Receive();
                }
                catch(Exception e)
                {
                    _logger.LogError($"No can has read: {e.Message}");
                }

                await Task.Delay(100, stoppingToken);
            }
        }
    }
}
