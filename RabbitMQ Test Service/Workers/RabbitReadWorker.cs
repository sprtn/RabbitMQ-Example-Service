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
        private readonly string QueueName = "SimpleMessage";
        private readonly string RabbitHost;

        public RabbitReadWorker(ILogger<RabbitPublishWorker> logger)
        {
            _logger = logger;
            RabbitHost = ConfigurationManager.AppSettings["RabbitQueue"];
            RabbitHost ??= "localhost";
            Receiver = new MQReceiver(RabbitHost);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Reader Worker running at: {time}", DateTimeOffset.Now);
                
                try
                {
                    Receiver.Receive(QueueName);
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
