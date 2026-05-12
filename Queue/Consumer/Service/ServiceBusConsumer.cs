
using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Amqp.Framing;
using System.Text;
using System.Text.Json;

namespace Consumer.Service
{
    public class ServiceBusConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        //private ServiceBusSessionProcessor _processor;
        public ServiceBusConsumer(IConfiguration configuration)
        {
            _configuration = configuration;

        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Console.WriteLine("Service Bus Consumer is starting.");
            var connectionString = _configuration["ServiceBus:ConnectionString"];
            var queueName = _configuration["ServiceBus:QueueName"];
            var client = new ServiceBusClient(connectionString);
            var _processor = client.CreateSessionProcessor(queueName, new ServiceBusSessionProcessorOptions
            {
                MaxConcurrentSessions = 1,
                MaxConcurrentCallsPerSession = 1,
                AutoCompleteMessages = false
            });

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync(cancellationToken);   
            //return base.StartAsync(cancellationToken);
        }

        private Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error : {args.Exception.Message}");
            return Task.CompletedTask;
        }

        private async Task MessageHandler(ProcessSessionMessageEventArgs args)
        {
            var body = args.Message.Body.ToString();
            Console.WriteLine($"Message from Azure Service Bus : {body}");
            await args.CompleteMessageAsync(args.Message);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            throw new NotImplementedException();
        }
    }
}
