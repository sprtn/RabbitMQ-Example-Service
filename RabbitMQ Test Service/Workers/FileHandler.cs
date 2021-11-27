using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ_Test_Service.Models;
using RabbitMQTestProgram.Handlers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RabbitMQ_Test_Service
{
    public class FileHandler : BackgroundService
    {
        private readonly ILogger<FileHandler> _logger;
        private readonly string QueueName = "SimpleMessage";
        private readonly MQPublisher Publisher;
        private readonly FileService FileService;

        public FileHandler(ILogger<FileHandler> logger)
        {
            var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
            Publisher = new MQPublisher(configuration);
            FileService = new FileService(configuration);
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                
                try
                {
                    List<SimpleMessage> simpMsgList = FileService.ScrapeIncommingSimpleMessages();
                    foreach (var simpMsg in simpMsgList)
                    {
                        simpMsg.Sender = "ReSender";
                        Publisher.Publish(QueueName, simpMsg);
                    }
                }
                catch(Exception e)
                {
                    _logger.LogError($"Unable to redistribute messages: {e.Message}");
                }

                await Task.Delay(500, stoppingToken);
            }
        }
    }
}
