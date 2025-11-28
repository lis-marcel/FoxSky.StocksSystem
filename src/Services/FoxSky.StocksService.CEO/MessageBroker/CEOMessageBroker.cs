using FoxSky.StocksService.CEO.Services;
using FoxSky.StocksSystem.SharedServices;
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
        private readonly string _dmsExchangeName;
        private readonly string _accountancyReportRequestRoutingKey;
        private readonly string _accountancyQueueName;
        private readonly string _reportNotificationRoutingKey;
        private readonly string _ceoQueueName;
        private readonly ICeoService _ceoService;
        private AsyncEventingBasicConsumer? _consumer;
        private bool _disposed;
        private CancellationTokenSource? _cancellationTokenSource;

        public CEOMessageBroker(ICeoService ceoService)
        {
            _ceoService = ceoService ?? throw new ArgumentNullException(nameof(ceoService));

            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI")
                ?? throw new InvalidOperationException("MESSAGE_BROKER_URI environment variable is not set");

            _ceoExchangeName = Environment.GetEnvironmentVariable("CEO_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("CEO_QUEUE_NAME environment variable is not set");

            _dmsExchangeName = Environment.GetEnvironmentVariable("DMS_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("CEO_QUEUE_NAME environment variable is not set");

            _ceoQueueName = Environment.GetEnvironmentVariable("CEO_QUEUE_NAME") 
                ?? throw new InvalidOperationException("CEO_QUEUE_NAME environment variable is not set");

            _accountancyReportRequestRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY")
                ?? throw new InvalidOperationException("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY environment variable is not set");

            _reportNotificationRoutingKey = Environment.GetEnvironmentVariable("REPORT_NOTIFICATION_ROUTING_KEY")
                ?? throw new InvalidOperationException("REPORT_NOTIFICATION_ROUTING_KEY environment variable is not set");

            var factory = new ConnectionFactory { Uri = new Uri(messageUri) };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("[CeoService] Initializing exchange and queue...");

            try
            {
                await _channel.QueueDeclareAsync(
                queue: _ceoQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

                Console.WriteLine($"[CeoService] Declared queue: {_ceoQueueName}");
            }
            catch
            (Exception ex)
            {
                Console.WriteLine($"[CeoService] Error during initialization: {ex.Message}");
            }

            try
            {
                await _channel.ExchangeDeclareAsync(
                exchange: _ceoExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

                Console.WriteLine($"[CeoService] Declared exchange: {_ceoExchangeName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CeoService] Error declaring queue: {ex.Message}");
            }

            try
            {
                await _channel.QueueBindAsync(
                    queue: _ceoQueueName,
                    exchange: _dmsExchangeName,
                    routingKey: _reportNotificationRoutingKey);

                Console.WriteLine($"[CeoService] Bound queue to DMS exchange with routing key: {_reportNotificationRoutingKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CeoService] Error binding queue: {ex.Message}");
            }

            Console.WriteLine("[CeoService] Message queue initialization completed");
        }

        public async Task PublishMessageAsync(string exchange, string receivcer, string message)
        {
            ThrowIfDisposed();
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: receivcer,
                body: body);

            Console.WriteLine($"[CeoService] Published message");
        }

        public async Task PublishMessageAsync(string exchange, string receivcer, byte[] message)
        {
            ThrowIfDisposed();

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: receivcer,
                body: message);

            Console.WriteLine($"[CeoService] Published message");
        }

        public async Task<OperationResult> ReceiveMessageAsync()
        {
            ThrowIfDisposed();
            _cancellationTokenSource = new CancellationTokenSource();
            var completionSource = new TaskCompletionSource<OperationResult>();

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (model, ea) => await Task.Run(async() =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                Console.WriteLine($"[CeoService] Received message with routing key: {routingKey}");
                try
                {
                    if (routingKey == _reportNotificationRoutingKey)
                    {
                        _ceoService.ReceiveReportData(message);
                    } 
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[CeoService] Error processing message: {ex.Message}");
                }
            });

            string consumerTag = await _channel.BasicConsumeAsync(
                queue: _ceoQueueName,
                autoAck: true,
                consumer: _consumer);

            Console.WriteLine($"[CeoService] Waiting for messages. Consumer tag: {consumerTag}");
            Console.WriteLine("[CEOServiceS] Press Ctrl+C to exit.");

            // Wait until cancellation is requested
            await Task.Run(() =>
            {
                try
                {
                    _cancellationTokenSource.Token.WaitHandle.WaitOne();
                    completionSource.SetResult(OperationResult.Succeeded("Message processing completed"));
                }
                catch (Exception ex)
                {
                    completionSource.SetResult(OperationResult.Failed($"Error during message processing: {ex.Message}"));
                }
            });

            return await completionSource.Task;
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
                    Console.WriteLine($"[CeoService] Error stopping consumer: {ex.Message}");
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

