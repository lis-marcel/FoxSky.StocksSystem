using FoxSky.StocksService.CEO.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FoxSky.StocksService.CEO.MessageBroker
{
    public class CEOMessageBroker : ICEOMessageBroker
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _ceoExchangeName;
        private readonly string _accountancyReportRequestRoutingKey;
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

            _ceoExchangeName = Environment.GetEnvironmentVariable("CEO_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("CEO_QUEUE_NAME environment variable is not set");

            _accountancyReportRequestRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY")
                ?? throw new InvalidOperationException("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY environment variable is not set");

            var factory = new ConnectionFactory { Uri = new Uri(messageUri) };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("[CEOService] Initializing exchange...");

            await _channel.ExchangeDeclareAsync(
                exchange: _ceoExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[CEOService] Declared exchange: {_ceoExchangeName}");

            Console.WriteLine("[CEOService] Message queue initialization completed");
        }

        public async Task PublishMessageAsync(string message)
        {
            ThrowIfDisposed();
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: _ceoExchangeName,
                routingKey: _accountancyReportRequestRoutingKey,
                body: body);

            Console.WriteLine($"[CEOService] Published message");
        }

        public async Task StopReceivingAsync()
        {
            if (_consumer != null)
            {
                try
                {
                    await _channel.BasicCancelAsync(_consumer.ConsumerTags.First());
                    _cancellationTokenSource?.Cancel();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CEOService] Error stopping consumer: {ex.Message}");
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(CEOMessageBroker));
        }

        public void Dispose()
        {
            if (_disposed) return;

            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
            _channel?.Dispose();
            _connection?.Dispose();

            _disposed = true;
        }

    }
}

