using Azure.Messaging.ServiceBus;
using System.Text;
using System.Text.Json;

namespace Producer.Services
{
    public class ServiceBusSenderService
    {
        private readonly IConfiguration _configuration;
        public ServiceBusSenderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendMessageAsync(object data) //data {orderid:1, message:"Test Message"}
        {
            //var option = new JsonSerializerOptions
            //{
            //    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //    PropertyNameCaseInsensitive = true,
            //    WriteIndented = true
            //};
            var connectionString = _configuration["ServiceBus:ConnectionString"];
            var queueName = _configuration["ServiceBus:QueueName"];
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queueName);

            var json = JsonSerializer.Serialize(data);

            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(json))
            {
                SessionId = "1001"
            };
            await sender.SendMessageAsync(message);

            await sender.DisposeAsync(); 
            await client.DisposeAsync();
        }
    }
}
