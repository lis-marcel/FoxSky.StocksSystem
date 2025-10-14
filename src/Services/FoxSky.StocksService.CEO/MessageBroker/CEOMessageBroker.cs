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
    }
}
