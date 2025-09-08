using RabbitMQ.Client;
using System.Text;

namespace FoxSky.StocksSystem.StocksProvider.MessageQueue
{
    internal class MessageQueue : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly string _queueName;

        public static async Task<MessageQueue> CreateAsync()
        {
            var messageQueue = new MessageQueue();
            await messageQueue.InitializeAsync();
            return messageQueue;
        }

        private MessageQueue()
        {
            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI");
            var clientName = Environment.GetEnvironmentVariable("MESSAGE_BROKER_CLIENT_NAME");
            _exchangeName = Environment.GetEnvironmentVariable("STOCKS_EXCHANGE")!;
            _routingKey = Environment.GetEnvironmentVariable("ROUTING_KEY")!;
            _queueName = Environment.GetEnvironmentVariable("QUEUE_NAME")!;

            if (string.IsNullOrEmpty(messageUri) || string.IsNullOrEmpty(clientName) ||
                string.IsNullOrEmpty(_exchangeName) || string.IsNullOrEmpty(_routingKey) ||
                string.IsNullOrEmpty(_queueName))
                throw new ArgumentNullException("Message broker configuration is not set in environment variables.");

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(messageUri),
                ClientProvidedName = clientName
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        private async Task InitializeAsync()
        {
            await ConfigureMessageBrokerAsync();
        }

        private async Task ConfigureMessageBrokerAsync()
        {
            await _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Direct,
                durable: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueBindAsync(
                queue: _queueName,
                exchange: _exchangeName,
                routingKey: _routingKey);
        }

        public async Task PublishMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

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