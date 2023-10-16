using Azure.Messaging.ServiceBus;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace InfoSafeReceiver.API.Messaging
{
    public class RmqServiceBusConsumer : IHostedService
    {
        private readonly IConfiguration _configuration;

        private readonly EventingBasicConsumer _consumer;

        public RmqServiceBusConsumer(
            IConfiguration configuration)
        {
            _configuration = configuration;

            var serviceBusConnectionString = _configuration.GetConnectionString("RMQConnectionString");
            var factory = new ConnectionFactory() { HostName = serviceBusConnectionString };
            var connection = factory.CreateConnection();
            var channel = connection.CreateModel();

            channel.QueueDeclare("BasicTest", false, false, false, null);
            _consumer = new EventingBasicConsumer(channel);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await Task.Run(() =>
            {
                _consumer.Received += ProcessContactMessageAsync;
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        private void ProcessContactMessageAsync(object? sender, BasicDeliverEventArgs e)
        {
            var body = e.Body.Span;
            var message = Encoding.UTF8.GetString(body);
        }

        //private async Task ProcessContactMessageAsync(ProcessMessageEventArgs args)
        //{
        //    try
        //    {
        //        var message = args.Message.Body.ToString();

        //        await args.CompleteMessageAsync(args.Message);
        //    }
        //    catch (Exception ex)
        //    {
        //        //Complete, Abandon, Dead-lettter, Defer
        //        if (args.Message.DeliveryCount > 5)
        //        {
        //            await args.DeadLetterMessageAsync(args.Message, ex.Message, ex.ToString());
        //        }
        //    }
        //}
    }
}
