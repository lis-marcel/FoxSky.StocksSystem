using FoxSky.StocksService.CEO.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FoxSky.StocksService.CEO.MessageBroker
{
    public class CEOMessageBroker : ICEOMessageBroker
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _accountancyExchangeName;
        private readonly string _accountancyDataRoutingKey;
        private readonly string _accountancyQueueName;
        private readonly ICEOService _ceoService;
        private AsyncEventingBasicConsumer? _consumer;
        private bool _disposed;
        private CancellationTokenSource? _cancellationTokenSource;

        public CEOMessageBroker(ICEOService ceoService)
        {
            _ceoService = ceoService ?? throw new ArgumentNullException(nameof(ceoService));

            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI")
                ?? throw new InvalidOperationException("MESSAGE_BROKER_URI environment variable is not set");

            _accountancyExchangeName = Environment.GetEnvironmentVariable("ACCOUNTANCY_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("ACCOUNTANCY_EXCHANGE_NAME environment variable is not set");

            _accountancyQueueName = Environment.GetEnvironmentVariable("ACCOUNTANCY_QUEUE_NAME")
                ?? throw new InvalidOperationException("ACCOUNTANCY_QUEUE_NAME environment variable is not set");

            _accountancyDataRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_DATA_ROUTING_KEY")
                ?? throw new InvalidOperationException("ACCOUNTANCY_DATA_ROUTING_KEY environment variable is not set");

            var factory = new ConnectionFactory { Uri = new Uri(messageUri) };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("[CEOService] Initializing message queues and exchanges...");

            await _channel.QueueDeclareAsync(
                queue: _accountancyQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[CEOService] Declared queue: {_accountancyQueueName}");

            await _channel.ExchangeDeclareAsync(
                exchange: _accountancyExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[CEOService] Declared exchange: {_accountancyExchangeName}");

            // Bind to traders exchange
            try
            {
                await _channel.QueueBindAsync(
                    queue: _accountancyQueueName,
                    exchange: _tradersExchangeName,
                    routingKey: _accountancyDataRoutingKey);

                Console.WriteLine($"[CEOService] Bound queue to traders exchange with routing key: {_accountancyDataRoutingKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CEOService] Error binding to traders exchange: {ex.Message}");
            }

            Console.WriteLine("[CEOService] Message queue initialization completed");
        }

    }
}
}
