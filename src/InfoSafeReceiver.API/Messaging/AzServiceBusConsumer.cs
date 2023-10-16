using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Configuration;

namespace InfoSafeReceiver.API.Messaging
{
    public class AzServiceBusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;

        private readonly ServiceBusProcessor contactMessageProcessor;

        public AzServiceBusConsumer(
            IConfiguration configuration)
        {
            _configuration = configuration;

            var serviceBusConnectionString = _configuration.GetConnectionString("AzBusConnectionString");
            var client = new ServiceBusClient(serviceBusConnectionString);

            var options = new ServiceBusProcessorOptions()
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
                //SubQueue = SubQueue.DeadLetter
            };
            contactMessageProcessor = client.CreateProcessor("contactsavedmessagetopic", "InfoSafeContactSubscription", options);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            contactMessageProcessor.ProcessMessageAsync += ProcessContactMessageAsync;
            contactMessageProcessor.ProcessErrorAsync += ProcessErrorAsync;

            await contactMessageProcessor.StartProcessingAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await contactMessageProcessor.StopProcessingAsync();
            await contactMessageProcessor.CloseAsync();
        }

        private async Task ProcessContactMessageAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message.Body.ToString();

                await args.CompleteMessageAsync(args.Message);
            }
            catch (Exception ex)
            {
                //Complete, Abandon, Dead-lettter, Defer
                if (args.Message.DeliveryCount > 5)
                {
                    await args.DeadLetterMessageAsync(args.Message, ex.Message, ex.ToString());
                }
            }
        }

        private async Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            //throw new NotImplementedException();
        }
    }
}
