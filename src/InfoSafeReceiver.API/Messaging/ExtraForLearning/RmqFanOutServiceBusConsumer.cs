using InfoSafeReceiver.API.Messages;
using InfoSafeReceiver.API.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.Extensions;
using System.Text;
using System.Threading.Channels;

namespace InfoSafeReceiver.API.Messaging.ExtraForLearning
{
    public class RmqFanOutServiceBusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _services;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RmqFanOutServiceBusConsumer(
            IConfiguration configuration,
            IServiceScopeFactory services)
        {
            _configuration = configuration;
            _services = services;

            var serviceBusConnectionString = _configuration.GetConnectionString("RMQConnectionString");
            var factory = new ConnectionFactory() { HostName = serviceBusConnectionString };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.ExchangeDeclare("ContactSavedMessageTopic", ExchangeType.Fanout);
            _channel.QueueDeclare("InfoSafeContactSubscription", false, false, false, null);
            _channel.QueueBind("InfoSafeContactSubscription", "ContactSavedMessageTopic", string.Empty);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += ProcessContactMessage;

            _channel.BasicConsume("InfoSafeContactSubscription", true, consumer);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _channel.Close();
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
