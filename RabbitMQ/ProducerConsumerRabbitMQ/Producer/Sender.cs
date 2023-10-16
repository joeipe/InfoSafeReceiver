// See https://aka.ms/new-console-template for more information
using RabbitMQ.Client;
using System.Text;

var factory = new ConnectionFactory() { HostName = "localhost" };
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
    channel.QueueDeclare("BasicTest", false, false, false, null);

    var message = "Just a test message by JI";
    var body = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("", "BasicTest", null, body);
    Console.WriteLine($"Sent message {message}");
}

Console.WriteLine("Press [enter] to exit the Sender App...");
Console.ReadLine();
