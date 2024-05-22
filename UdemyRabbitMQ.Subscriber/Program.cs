// See https://aka.ms/new-console-template for more information


using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Reflection;
using System.Text;

var factory = new ConnectionFactory(); // ConnectionFactory sınıfından factory adında nesne oluşturduk.
factory.Uri = new Uri("amqps://dvunsgiv:L_l7SXKm58FN8dA6e_hjDRnMj2jDaRqQ@moose.rmq.cloudamqp.com/dvunsgiv"); // ConnectionFactory sınıfında Uri propertysine cloud tarafındaki adresi tanımladık.

using var connection = factory.CreateConnection(); // factory nesnesi üzerinden ConnectionFactory sınıfındaki CreateConnection metodunu çağırarak connection açtık.

var channel = connection.CreateModel(); // RabbitMQ'ya bu kanal üzerinden bağlanacağız.

channel.QueueDeclare("hello-queue", true, false, false); // Kanal üzerinde kuyruk oluşturduk. Name, durable, exclusive, autoDelete propertylerin true,false durumlarını belirledik.

var subscriber=new EventingBasicConsumer(channel); // EventingBasicConsumer ile bir tüketici oluşturulur ve channel parametresi ile bu tüketicinin hangi kanalı kullanacağı belirtilir.

channel.BasicConsume("hello-queue",true,subscriber); // channel üzerinden BasicConsume metodunu çağırdık. BasicConsume, metodundaki autoAck parametresi true yapıldı. Çünkü mesaj, subscriber                                                                tarafından teslim alınınca mesajı kuyruktan sil. RealTime'da bu false yapılır. yani biz istediğimiz zaman silinsin


subscriber.Received += (object? sender, BasicDeliverEventArgs e) =>
{
    var message=Encoding.UTF8.GetString(e.Body.ToArray());
    Console.WriteLine("Gelen Mesaj: " + message);
};



Console.ReadLine();
