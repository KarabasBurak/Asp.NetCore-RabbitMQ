using RabbitMQ.Client;

namespace RabbitMQWeb.Watermark.Services
{
    public class RabbitMQClientService:IDisposable
    {
        private readonly ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        public static string ExchangeName = "ImageDirectExchange";
        public static string RoutingWatermark = "watermark-route-image";
        public static string QueueName = "queue-watermark-image";
        private readonly ILogger<RabbitMQClientService> _logger;


        public RabbitMQClientService(ConnectionFactory connectionFactory, ILogger<RabbitMQClientService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public IModel Connect()
        {
            _connection = _connectionFactory.CreateConnection(); // Bağlantı oluşturuldu

            if (_channel is { IsOpen:true}) // Kanal bağlantısı var mı kontrol edildi.
            {
                return _channel;
            }

            _channel = _connection.CreateModel(); // Kanalın bağlantısı oluşturuldu

            _channel.ExchangeDeclare(ExchangeName, type: "direct", true, false);    // Exchange tanımlaması yapıldı

            _channel.QueueDeclare(QueueName, true, false, false, null);  // Kuyruk tanımlaması yapıldı

            _channel.QueueBind(exchange:ExchangeName,queue:QueueName,routingKey:RoutingWatermark);

            _logger.LogInformation("RabbitMQ ile bağlantı kuruldu ...");

            return _channel;
        }

        public void Dispose()
        {
            _channel?.Close();
            _channel?.Dispose();

            _connection?.Close();
            _connection?.Dispose();

            _logger.LogInformation("RabbitMQ ile bağlantı koptu ...");
        }
    }
}
