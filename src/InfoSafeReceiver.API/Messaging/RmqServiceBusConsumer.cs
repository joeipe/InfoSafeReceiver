using InfoSafeReceiver.API.Messages;
using InfoSafeReceiver.API.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.Extensions;
using System.Text;

namespace InfoSafeReceiver.API.Messaging
{
    public class RmqServiceBusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _services;

        private readonly IConnection _connection;
        private readonly IModel _contactMessageChannel;

        public RmqServiceBusConsumer(
            IConfiguration configuration,
            IServiceScopeFactory services)
        {
            _configuration = configuration;
            _services = services;

            var serviceBusConnectionString = _configuration.GetConnectionString("RMQConnectionString");
            var factory = new ConnectionFactory() { HostName = serviceBusConnectionString };
            _connection = factory.CreateConnection();
            _contactMessageChannel = _connection.CreateModel();

            _contactMessageChannel.QueueDeclare("ContactSavedMessageTopic", false, false, false, null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var contactMessageConsumer = new EventingBasicConsumer(_contactMessageChannel);
            contactMessageConsumer.Received += ProcessContactMessage;
            _contactMessageChannel.BasicConsume("ContactSavedMessageTopic", true, contactMessageConsumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _contactMessageChannel.Close();
            _connection.Close();

            return Task.CompletedTask;
        }

        private void ProcessContactMessage(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var value = message.OutputObject<ContactMessage>();

                using (var scope = _services.CreateScope())
                {
                    var messagingService = scope.ServiceProvider.GetRequiredService<MessagingService>();
                    Task.FromResult(messagingService.AddContactAsync(value));
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}