using Azure.Messaging.ServiceBus;
using InfoSafeReceiver.API.Messages;
using InfoSafeReceiver.API.Services;
using SharedKernel.Extensions;

namespace InfoSafeReceiver.API.Messaging
{
    public class AzServiceBusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _services;

        private readonly ServiceBusClient _client;
        private readonly List<IAsyncDisposable> _disposables = new();

        public AzServiceBusConsumer(
            IConfiguration configuration,
            IServiceScopeFactory services)
        {
            _configuration = configuration;
            _services = services;

            var serviceBusConnectionString = _configuration.GetConnectionString("AzBusConnectionString");
            _client = new ServiceBusClient(serviceBusConnectionString);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await StartBasicConsumeAsync("InfoSafeContactSubscription", "ContactSavedMessageTopic", async message =>
            {
                var value = message.OutputObject<ContactMessage>();
                using (var scope = _services.CreateScope())
                {
                    var messagingService = scope.ServiceProvider.GetRequiredService<MessagingService>();
                    await messagingService.AddContactAsync(value);
                }
            });
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            foreach (var disposable in _disposables)
            {
                await disposable.DisposeAsync();
            }
            await _client.DisposeAsync();
        }

        private async Task StartBasicConsumeAsync(string subscription, string topic, Func<string, Task> handleAsync)
        {
            var options = new ServiceBusProcessorOptions()
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1,
                MaxAutoLockRenewalDuration = TimeSpan.FromMinutes(10),
                //SubQueue = SubQueue.DeadLetter
            };
            var processor = _client.CreateProcessor(topic, subscription, options);
            _disposables.Add(processor);

            processor.ProcessMessageAsync += async (args) =>
            {
                try
                {
                    var message = args.Message.Body.ToString();

                    await handleAsync(message);

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
            };

            processor.ProcessErrorAsync += (args) =>
            {
                return Task.CompletedTask;
            };

            await processor.StartProcessingAsync();
        }
    }
}