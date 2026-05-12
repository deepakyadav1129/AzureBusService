using Azure.Messaging.ServiceBus;

namespace Producer.Services
{
    public class DlqService
    {
        public readonly IConfiguration _configuration;
        public DlqService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        private ServiceBusClient GetClient() =>
            new ServiceBusClient(_configuration["ServiceBus:ConnectionString"]);

        public async Task<List<object>> GetDlqMessageAsync()
        {
             var client = GetClient();

            var reciver1 = client.CreateReceiver($"{_configuration["ServiceBus:QueueName"]}/$DeadLetterQueue", new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter
            });

            var receiver = client.CreateReceiver(_configuration["ServiceBus:QueueName"], new ServiceBusReceiverOptions
            {
                SubQueue = SubQueue.DeadLetter
            });
            var messages = await receiver.ReceiveMessagesAsync(maxMessages: 10);
            var dlqMessages = new List<object>();
            foreach (var message in messages)
            {
               // var body = message.Body.ToString();
                dlqMessages.Add(new
                {
                    Id = message.MessageId,
                    reason = message.DeadLetterReason,
                    Description = message.DeadLetterErrorDescription,
                    Body = message.Body.ToString()
                });
                await receiver.CompleteMessageAsync(message);
            }
            await receiver.DisposeAsync();
            await client.DisposeAsync();
            return dlqMessages;
        }
    }
}
