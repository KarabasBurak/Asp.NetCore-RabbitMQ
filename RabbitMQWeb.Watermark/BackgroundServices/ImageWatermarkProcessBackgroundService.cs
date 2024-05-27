
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQWeb.Watermark.Services;
using System.Drawing;
using System.Text;
using System.Text.Json;

namespace RabbitMQWeb.Watermark.BackgroundServices
{
    public class ImageWatermarkProcessBackgroundService : BackgroundService
    {
        private readonly RabbitMQClientService _rabbitMQClientService;
        private readonly ILogger<ImageWatermarkProcessBackgroundService> _logger;
        private IModel _channel;
        public ImageWatermarkProcessBackgroundService(ILogger<ImageWatermarkProcessBackgroundService> logger, RabbitMQClientService rabbitMQClientService)
        {

            _logger = logger;
            _rabbitMQClientService = rabbitMQClientService;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _channel=_rabbitMQClientService.Connect();

            _channel.BasicQos(0, 1, false);

            return base.StartAsync(cancellationToken);
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var consumer=new AsyncEventingBasicConsumer(_channel); // Kanal üzerinden mesajı alacak consumer nesnesi oluşturuldu. consumer nesnesini, AsyncEventingBasicConsumer sınıfından türettik.

            _channel.BasicConsume(RabbitMQClientService.QueueName,false, consumer); // Event'i dinlemek için BasicConsume metodunu çağırdık

            consumer.Received += Consumer_Received;

            return Task.CompletedTask;
        }

        private Task Consumer_Received(object sender, BasicDeliverEventArgs @event)
        {
            try
            {
                var productImageCreatedEvent = JsonSerializer.Deserialize<ProductImageCreatedEvent>(Encoding.UTF8.GetString(@event.Body.ToArray()));

                var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Images", productImageCreatedEvent.ImageName);

                var siteName = "www.burakkarabas.com";

                using var img = Image.FromFile(path);

                using var graphic = Graphics.FromImage(img);

                var font = new Font(FontFamily.GenericMonospace, 40, FontStyle.Bold, GraphicsUnit.Pixel);

                var textSize = graphic.MeasureString(siteName, font);

                var color = Color.FromArgb(128, 255, 255, 255);

                var brush = new SolidBrush(color);

                var position = new Point(img.Width - ((int)textSize.Width + 30), img.Height - ((int)textSize.Height + 30));

                graphic.DrawString(siteName, font, brush, position);

                img.Save("wwwroot/Images/watermarks" + productImageCreatedEvent.ImageName);

                img.Dispose();

                graphic.Dispose();

                _channel.BasicAck(@event.DeliveryTag, false);
            }
            catch (Exception)
            {

                throw;
            }

           return Task.CompletedTask;

        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}
