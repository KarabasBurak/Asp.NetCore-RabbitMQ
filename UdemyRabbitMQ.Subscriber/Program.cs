using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shared;
using System;
using System.IO;
using System.Text;
using System.Text.Json;
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
            channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers);
            channel.BasicQos(0, 1, false);
            var subscriber = new EventingBasicConsumer(channel);
            var queueName = channel.QueueDeclare().QueueName;

            var headers = new Dictionary<string, object>
            {
                { "format", "pdf" },
                { "shape", "a4" },
                { "x-match", "any" },

            };

            channel.QueueBind(queueName,"header-exchange",String.Empty,headers);
            channel.BasicConsume(queueName, false, subscriber);

            Console.WriteLine("Loglar dinleniyor...");
             
            subscriber.Received += (sender, e) =>
            {
                var message = Encoding.UTF8.GetString(e.Body.ToArray());
                Product product = JsonSerializer.Deserialize<Product>(message);
                Thread.Sleep(1500);
                Console.WriteLine($"Gelen Mesaj: {product.Id}-{product.Name}-{product.Price}-{product.Stock}");
                channel.BasicAck(e.DeliveryTag, false);
            };

            Console.ReadLine();
        }
    }
}
