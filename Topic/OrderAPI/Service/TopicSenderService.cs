using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;

namespace OrderAPI.Service
{
    public class TopicSenderService
    {
        private readonly IConfiguration _configuration;

        public TopicSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task PublishOrderAsync(object message)
        {
            var serviceBusConnectionString = _configuration["ServiceBus:ConnectionString"];
            var topicName = _configuration["ServiceBus:TopicName"];
            var client = new ServiceBusClient(serviceBusConnectionString);
            var sender = client.CreateSender(topicName);
            try
            {
                var json = JsonSerializer.Serialize(message);
                var serviceBusMessage = new ServiceBusMessage(Encoding.UTF8.GetBytes(json));
                await sender.SendMessageAsync(serviceBusMessage);
            }
            finally
            {
                await sender.DisposeAsync();
                await client.DisposeAsync();
            }
        }                               
    }
}
