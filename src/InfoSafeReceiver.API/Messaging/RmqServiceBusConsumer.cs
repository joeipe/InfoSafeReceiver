using Azure.Messaging.ServiceBus;
using InfoSafeReceiver.API.Messages;
using InfoSafeReceiver.Application;
using InfoSafeReceiver.ViewModels;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using SharedKernel.Extensions;
using System.Text;
using System.Threading.Channels;

namespace InfoSafeReceiver.API.Messaging
{
    public class RmqServiceBusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;
        private readonly IServiceScopeFactory _services;

        private readonly IConnection _connection;
        private readonly IModel _channel;

        public RmqServiceBusConsumer(
            IConfiguration configuration,
            IServiceScopeFactory services)
        {
            _configuration = configuration;
            _services = services;

            var serviceBusConnectionString = _configuration.GetConnectionString("RMQConnectionString");
            var factory = new ConnectionFactory() { HostName = serviceBusConnectionString };
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            _channel.QueueDeclare("ContactSavedMessageTopic", false, false, false, null);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var consumer = new EventingBasicConsumer(_channel);

            consumer.Received += ProcessContactMessage;

            _channel.BasicConsume("ContactSavedMessageTopic", true, consumer);

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
            var body = e.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var value = message.OutputObject<ContactMessage>();
            var vm = new ContactVM
            {
                Id = 0,
                RefId = value.Id,
                FirstName = value.FirstName,
                LastName = value.LastName,
                DoB = value.DoB
            };

            using (var scope = _services.CreateScope())
            {
                var appService = scope.ServiceProvider.GetRequiredService<IAppService>();
                Task.FromResult(appService.AddContactAsync(vm));
            }
        }
    }
}
