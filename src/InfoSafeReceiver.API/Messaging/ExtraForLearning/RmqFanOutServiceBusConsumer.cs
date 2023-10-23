using InfoSafeReceiver.API.Messages;
using InfoSafeReceiver.API.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.Extensions;
using System.Text;

namespace InfoSafeReceiver.API.Messaging.ExtraForLearning
{
    public class RmqFanOutServiceBusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _services;

        private readonly IConnection _connection;

        public RmqFanOutServiceBusConsumer(
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
            StartBasicConsume("InfoSafeContactSubscription", "ContactSavedMessageTopic", async message =>
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

        private void StartBasicConsume(string queue, string exchange, Func<string, Task> handleAsync)
        {
            var channel = _connection.CreateModel();

            var dlxExchange = $"{exchange}_dlx";
            var dlxQueue = $"{queue}_dlx";
            channel.ExchangeDeclare(dlxExchange, ExchangeType.Fanout);
            channel.QueueDeclare(dlxQueue, false, false, false, null);
            channel.QueueBind(dlxQueue, dlxExchange, string.Empty);

            var args = new Dictionary<string, object>();
            args.Add("x-dead-letter-exchange", dlxExchange);
            channel.ExchangeDeclare(exchange, ExchangeType.Fanout);
            channel.QueueDeclare(queue, false, false, false, args);
            channel.QueueBind(queue, exchange, string.Empty);

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