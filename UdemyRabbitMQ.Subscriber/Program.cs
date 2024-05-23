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
            var subscriber = new EventingBasicConsumer(channel);
            var queueName = channel.QueueDeclare().QueueName;
            var routeKey = "Info.#";
            channel.QueueBind(queueName, "logs-topic",routeKey);
            channel.BasicConsume(queueName, false, subscriber);

            Console.WriteLine("Loglar dinleniyor...");
             
            subscriber.Received += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Thread.Sleep(1500);
                Console.WriteLine("Gelen Mesaj: " + message);
                // File.AppendAllText($"log-{logType.ToLower()}.txt", message + "\n");
                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
