using RabbitMQ.Client;
using System.Text;

namespace FoxSky.StocksSystem.StocksProvider.MessageQueueHandler
{
    internal class MessageQueueService : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;

        public static async Task<MessageQueueService> CreateAsync()
        {
            var messageQueue = new MessageQueueService();
            await messageQueue.ConfigureExchange();
            return messageQueue;
        }

        private MessageQueueService()
        {
            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI");
            _exchangeName = Environment.GetEnvironmentVariable("STOCKS_PROVIDER_EXCHANGE_NAME")!;
            _routingKey = Environment.GetEnvironmentVariable("STOCKS_DATA_ROUTING_KEY")!;

            if (string.IsNullOrEmpty(messageUri) ||
                string.IsNullOrEmpty(_exchangeName) || string.IsNullOrEmpty(_routingKey))
                throw new ArgumentNullException("Message broker configuration is not set in environment variables.");

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(messageUri),
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        private async Task ConfigureExchange()
        {
            await _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);
        }

        public async Task PublishMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: _routingKey,
                body: body);
        }

        public async Task PublishMessageAsync<T>(T obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: _exchangeName,
                routingKey: _routingKey,
                body: body);
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}