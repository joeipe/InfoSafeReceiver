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

        public RmqServiceBusConsumer(
            IConfiguration configuration,
            IServiceScopeFactory services)
        {
            _configuration = configuration;
            _services = services;

            var serviceBusConnectionString = _configuration.GetConnectionString("RMQConnectionString");
            var factory = new ConnectionFactory() { HostName = serviceBusConnectionString };
            _connection = factory.CreateConnection();
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            StartBasicConsume("InfoSafeContactSubscription", async message =>
            {
                var value = message.OutputObject<ContactMessage>();
                using (var scope = _services.CreateScope())
                {
                    var messagingService = scope.ServiceProvider.GetRequiredService<MessagingService>();
                    await messagingService.AddContactAsync(value);
                }
            });

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _connection.Close();

            return Task.CompletedTask;
        }

        private void StartBasicConsume(string queue, Func<string, Task> handleAsync)
        {
            var channel = _connection.CreateModel();

            channel.QueueDeclare(queue, false, false, false, null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    handleAsync(message).Wait();

                    channel.BasicAck(ea.DeliveryTag, false);
                }
                catch (Exception ex)
                {
                    channel.BasicNack(ea.DeliveryTag, false, false);
                }
            };
            channel.BasicConsume(queue, false, consumer);
        }
    }
}