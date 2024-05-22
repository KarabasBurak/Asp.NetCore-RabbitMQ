// See https://aka.ms/new-console-template for more information



using RabbitMQ.Client;
using RabbitMQ.Client.Exceptions;
using System.Text;

var factory = new ConnectionFactory(); // ConnectionFactory sınıfından factory adında nesne oluşturduk.
factory.Uri = new Uri("amqps://dvunsgiv:L_l7SXKm58FN8dA6e_hjDRnMj2jDaRqQ@moose.rmq.cloudamqp.com/dvunsgiv"); // ConnectionFactory sınıfında Uri propertysine cloud tarafındaki adresi tanımladık.

using var connection = factory.CreateConnection(); // factory nesnesi üzerinden ConnectionFactory sınıfındaki CreateConnection metodunu çağırarak connection açtık.

var channel = connection.CreateModel(); // RabbitMQ'ya bu kanal üzerinden bağlanacağız.

//channel.QueueDeclare("hello-queue", true, false, false); // Kanal üzerinde kuyruk oluşturduk. Name, durable, exclusive, autoDelete propertylerin true,false durumlarını belirledik.

channel.ExchangeDeclare("logs-fanout", durable: true, type: ExchangeType.Fanout); // üst satırda mesajları direkt kuyruğa gönderiyorduk. Burada Exchange üzerinden kuyruğa göndereceğiz. Fark bu

Enumerable.Range(1, 50).ToList().ForEach(x =>
{
    string message = $"log {x}";

    var messageBody = Encoding.UTF8.GetBytes(message);

    channel.BasicPublish("logs-fanout", " " , null, messageBody);

    Console.WriteLine($"Mesaj Gönderilmiştir : {message}");
});

string message = "hello world"; // Kuyruktaki mesajı tanımladık.

var messageBody=Encoding.UTF8.GetBytes(message);

channel.BasicPublish(string.Empty, "hello-queue", null, messageBody);

Console.WriteLine("Mesaj Gönderilmiştir.");

Console.ReadLine();


    /*
    RabbitMQ, uygulamalar arasında asenkron mesajlaşma ve işleme için kullanılan popüler bir mesaj kuyruğu sistemidir. Temel mantığı, mesajların güvenli, güvenilir ve verimli bir şekilde bir yerden başka bir yere iletilmesini sağlamaktır. RabbitMQ, mesajları geçici olarak saklar ve alıcıların (subscribers) onları almasını sağlar.

RabbitMQ'nun Temel Mantığı
* Mesajlar (Messages): RabbitMQ iletilmek üzere gönderilen verilerdir. Mesajlar herhangi bir formatta olabilir (JSON, XML, metin, vb.).
* Exchange (Değişim): Mesajların alındığı ve yönlendirildiği bileşendir. Exchange, mesajları belirli kurallara göre ilgili kuyruklara yönlendirir.
* Queue (Kuyruk): Mesajların geçici olarak saklandığı yerdir. Kuyruklar, mesajların tüketicilere (subscribers) iletilene kadar saklandığı yapıdır.
* Binding (Bağlantı): Exchange ile kuyruklar arasındaki ilişkiyi tanımlar. Mesajların hangi exchange'den hangi kuyruklara gideceğini belirler.
* Publisher (Yayıncı): Mesajları exchange'e gönderen bileşendir.
* Subscriber (Abone): Mesajları kuyruktan alıp işleyen bileşendir.
     */


/*
 Kanal üzerinden oluşturulan kuyrukta (QueueDeclare) name, durable, exclusive, autoDelete propertyler yer almaktadır. 
Durable; false yaparsak RabbitMQ'deki kuyruklar memory'de tutulur. RabbitMQ restart atarsa kutuklar silinir. True yaparsak kuyruklar fiziksel olarak tutulur ve RabbitMQ restart atsa bile silinmez.

Exclusive; Oluşturduğumuz kuyruğa farklı kanallardan veya proseslerden bağlanabilmek veya erişebilmek için false set edilir. True yaparsak sadece publisher üzerinden erişilebilir.

autoDelete; True yaparsak Subcriber yanlışlıkla down olursa kuyruk otomatik olarak silinir. False yaparsak subscriber kapansa bile kuyruk silinmez
 */