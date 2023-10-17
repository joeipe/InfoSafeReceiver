﻿using Azure.Messaging.ServiceBus;
using InfoSafeReceiver.API.Messages;
using InfoSafeReceiver.API.Services;
using SharedKernel.Extensions;

namespace InfoSafeReceiver.API.Messaging
{
    public class AzServiceBusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _services;

        private readonly ServiceBusProcessor _contactMessageProcessor;

        public AzServiceBusConsumer(
            IConfiguration configuration,
            IServiceScopeFactory services)
        {
            _configuration = configuration;
            _services = services;

            var serviceBusConnectionString = _configuration.GetConnectionString("AzBusConnectionString");
            var client = new ServiceBusClient(serviceBusConnectionString);

            var options = new ServiceBusProcessorOptions()
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
                //SubQueue = SubQueue.DeadLetter
            };
            _contactMessageProcessor = client.CreateProcessor("contactsavedmessagetopic", "InfoSafeContactSubscription", options);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            _contactMessageProcessor.ProcessMessageAsync += ProcessContactMessageAsync;
            _contactMessageProcessor.ProcessErrorAsync += ProcessErrorAsync;

            await _contactMessageProcessor.StartProcessingAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _contactMessageProcessor.StopProcessingAsync();
            await _contactMessageProcessor.CloseAsync();
        }

        private async Task ProcessContactMessageAsync(ProcessMessageEventArgs args)
        {
            try
            {
                var message = args.Message.Body.ToString();

                var value = message.OutputObject<ContactMessage>();
                using (var scope = _services.CreateScope())
                {
                    var messagingService = scope.ServiceProvider.GetRequiredService<MessagingService>();
                    await messagingService.AddContactAsync(value);
                }

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