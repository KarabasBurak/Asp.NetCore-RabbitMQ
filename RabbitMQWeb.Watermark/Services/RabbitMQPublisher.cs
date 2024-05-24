using RabbitMQ.Client;
using System.Text;
using System.Text.Json;
using System.Text.Unicode;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMQPublisher
    {
        private readonly RabbitMQClientService _rabbitmqClientService;

        public RabbitMQPublisher(RabbitMQClientService rabbitmqClientService)
        {
            _rabbitmqClientService = rabbitmqClientService;
        }

        public void Publish(ProductImageCreatedEvent productImageCreatedEvent)
        {
            var channel = _rabbitmqClientService.Connect(); // RabbitMQClientService sınıfına bağlandık.

            // ProductImageCreatedEvent sınıfında tanımlanan ImageName propertysini productImageCreatedEvent nesnesi ile json formatına dönüştürdük. bodyString değişkenine atadık.
            var bodyString =JsonSerializer.Serialize(productImageCreatedEvent); 

            var bodyByte= Encoding.UTF8.GetBytes(bodyString); // bodyString içindeki karakterleri, UTF-8 formatında bayt dizisine dönüştürdük.

            //properties değişkeni RabbitMQ'nun IBasicProperties arayüzünü uygulayan bir nesne örneğidir. Mesaja özellik atayabiliyoruz.
            var properties=channel.CreateBasicProperties();

            properties.Persistent = true; // mesaja kalıcılık özelliğini sağladık

            channel.BasicPublish(exchange: RabbitMQClientService.ExchangeName, routingKey: RabbitMQClientService.RoutingWatermark, basicProperties: properties, body: bodyByte);
        }
    }
}




// RabbitMQPublisher sınıfında, RabbitMQ ile mesajın gönderilecek kısmını düzenliyoruz.
