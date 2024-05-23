using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace UdemyRabbitMQ.Subscriber
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory
            {
                Uri = new Uri("amqps://dvunsgiv:L_l7SXKm58FN8dA6e_hjDRnMj2jDaRqQ@moose.rmq.cloudamqp.com/dvunsgiv")
            };

            using var connection = factory.CreateConnection();
            var channel = connection.CreateModel();
            channel.BasicQos(0, 1, false);

            var logTypes = new string[] { "Critical", "Error", "Warning", "Info" };

            foreach (var logType in logTypes)
            {
                var queueName = $"direct-queue-{logType}";
                var subscriber = new EventingBasicConsumer(channel);
                subscriber.Received += (sender, e) =>
                {
                    var message = Encoding.UTF8.GetString(e.Body.ToArray());
                    Thread.Sleep(1500);
                    Console.WriteLine($"Gelen Mesaj: {message}");
                    // File.AppendAllText($"log-{logType.ToLower()}.txt", message + "\n");
                    channel.BasicAck(e.DeliveryTag, false);
                };

                channel.BasicConsume(queueName, false, subscriber);
            }

            Console.WriteLine("Loglar dinleniyor...");
            Console.ReadLine();
        }
    }
}
