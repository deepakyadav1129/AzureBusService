
using Azure.Messaging.ServiceBus;

namespace AnalyticsAPI.Consumers
{
    public class OrderCreatedConsumer : BackgroundService
    {
        private readonly IConfiguration _configuration;
        private ServiceBusProcessor _processor;

        public OrderCreatedConsumer(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var client = new ServiceBusClient(_configuration["ServiceBus:ConnectionString"]);
            _processor = client.CreateProcessor(_configuration["ServiceBus:TopicName"], _configuration["ServiceBus:SubscriptionName"], new ServiceBusProcessorOptions());

            _processor.ProcessMessageAsync += MessageHandler;
            _processor.ProcessErrorAsync += ErrorHandler;
            await _processor.StartProcessingAsync(cancellationToken);
            //return base.StartAsync(cancellationToken);
        }

        private async Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($"Error occurred: {args.Exception.Message}");
                await Task.CompletedTask;   
        }

        private async Task MessageHandler(ProcessMessageEventArgs args)
        {
             Console.WriteLine("Analytics Data : "+args.Message.Body.ToString());
            await args.CompleteMessageAsync(args.Message);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
           return Task.CompletedTask;
        }
    }
}
