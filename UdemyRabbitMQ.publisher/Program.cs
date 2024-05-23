using RabbitMQ.Client;
using Shared;
using System.Text;
using System.Text.Json;

public enum LogNames
{
    Critical=1,
    Error=2,
    Warning=3,
    Info=4
}

class Program
{
    static void Main(string[] args)
    {

        var factory = new ConnectionFactory(); // ConnectionFactory sınıfından factory adında nesne oluşturduk.

        factory.Uri = new Uri("amqps://dvunsgiv:L_l7SXKm58FN8dA6e_hjDRnMj2jDaRqQ@moose.rmq.cloudamqp.com/dvunsgiv"); // ConnectionFactory sınıfında Uri propertysine cloud tarafındaki adresi tanımladık.

        using var connection = factory.CreateConnection(); // factory nesnesi üzerinden ConnectionFactory sınıfındaki CreateConnection metodunu çağırarak connection açtık.

        var channel = connection.CreateModel(); // RabbitMQ'ya bu kanal üzerinden bağlanacağız.

        //channel.QueueDeclare("hello-queue", true, false, false); // Kanal üzerinde kuyruk oluşturduk. Name, durable, exclusive, autoDelete propertylerin true,false durumlarını belirledik.

        channel.ExchangeDeclare("header-exchange", durable: true, type: ExchangeType.Headers); // üst satırda mesajları direkt kuyruğa gönderdik. Burada Exchange üzerinden kuyruğa göndereceğiz.

        var headers = new Dictionary<string, object>
        {
            { "format", "pdf" },
            { "shape", "a4" }
        };


        var properties =channel.CreateBasicProperties();
        properties.Headers=headers;
        properties.Persistent = true; // RabbitMQ restart edilse bile bu kod ile mesajlar kaybolmayacak

        var product = new Product 
        { 
            Id = 1, Name = "Laptop",
            Price = 35000,
            Stock = 1500 
        };

        var productJsonString=JsonSerializer.Serialize(product);

        channel.BasicPublish("header-exchange", string.Empty, properties, Encoding.UTF8.GetBytes(productJsonString));

        Console.WriteLine("Mesaj Gönderilmiştir");

        Console.ReadLine();
    }
}
















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