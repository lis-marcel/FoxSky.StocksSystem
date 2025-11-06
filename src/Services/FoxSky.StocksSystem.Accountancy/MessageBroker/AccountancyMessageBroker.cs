using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Services;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FoxSky.StocksSystem.Accountancy.MessageBroker
{
    internal class AccountancyMessageBroker : IAccountancyMessageBroker, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _stocksProviderExchangeName;
        private readonly string _accountancyExchangeName;
        private readonly string _accountancyDataRoutingKey;
        private readonly string _ceoDataRoutingKey;
        private readonly string _accountancyQueueName;
        private readonly string _tradersExchangeName;
        private readonly string _ceoExchangeName;
        private readonly string _accountancyReportRequestRoutingKey;
        private readonly string _dmsReportRoutingKey;
        private readonly IAccountancyService _accountancyService;
        private AsyncEventingBasicConsumer? _consumer;
        private bool _disposed;
        private CancellationTokenSource? _cancellationTokenSource;

        public AccountancyMessageBroker(IAccountancyService accountancyService)
        {
            _accountancyService = accountancyService ?? throw new ArgumentNullException(nameof(accountancyService));

            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI")
                ?? throw new InvalidOperationException("MESSAGE_BROKER_URI environment variable is not set");

            _accountancyExchangeName = Environment.GetEnvironmentVariable("ACCOUNTANCY_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("ACCOUNTANCY_EXCHANGE_NAME environment variable is not set");

            _stocksProviderExchangeName = Environment.GetEnvironmentVariable("STOCKS_PROVIDER_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("STOCKS_PROVIDER_EXCHANGE_NAME environment variable is not set");

            _ceoDataRoutingKey = Environment.GetEnvironmentVariable("CEO_DATA_ROUTING_KEY")
                ?? throw new InvalidOperationException("CEO_DATA_ROUTING_KEY environment variable is not set");

            _accountancyQueueName = Environment.GetEnvironmentVariable("ACCOUNTANCY_QUEUE_NAME")
                ?? throw new InvalidOperationException("ACCOUNTANCY_QUEUE_NAME environment variable is not set");

            _accountancyDataRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_DATA_ROUTING_KEY")
                ?? throw new InvalidOperationException("ACCOUNTANCY_DATA_ROUTING_KEY environment variable is not set");

            _tradersExchangeName = Environment.GetEnvironmentVariable("TRADERS_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("TRADERS_EXCHANGE_NAME environment variable is not set");

            _accountancyReportRequestRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY")
                ?? throw new InvalidOperationException("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY environment variable is not set");

            _ceoExchangeName = Environment.GetEnvironmentVariable("CEO_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("CEO_EXCHANGE_NAME environment variable is not set");

            _dmsReportRoutingKey = Environment.GetEnvironmentVariable("DMS_REPORT_ROUTING_KEY")
                ?? throw new InvalidOperationException("DMS_REPORT_ROUTING_KEY environment variable is not set");

            var factory = new ConnectionFactory { Uri = new Uri(messageUri) };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("[AccountancyService] Initializing message queues and exchanges...");

            await _channel.QueueDeclareAsync(
                queue: _accountancyQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[AccountancyService] Declared queue: {_accountancyQueueName}");

            await _channel.ExchangeDeclareAsync(
                exchange: _accountancyExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[AccountancyService] Declared exchange: {_accountancyExchangeName}");

            // Bind to StocksProvider exchange
            try
            {
                await _channel.QueueBindAsync(
                    queue: _accountancyQueueName,
                    exchange: _stocksProviderExchangeName,
                    routingKey: _ceoDataRoutingKey);

                Console.WriteLine($"[AccountancyService] Bound queue to stocks provider exchange with routing key: {_ceoDataRoutingKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error binding to stocks provider exchange: {ex.Message}");
            }

            // Bind to Traders exchange
            try
            {
                await _channel.QueueBindAsync(
                    queue: _accountancyQueueName,
                    exchange: _tradersExchangeName,
                    routingKey: _accountancyDataRoutingKey);

                Console.WriteLine($"[AccountancyService] Bound queue to traders exchange with routing key: {_accountancyDataRoutingKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error binding to traders exchange: {ex.Message}");
            }

            // Bind to CEOs exchange
            try
            {
                await _channel.QueueBindAsync(
                    queue: _accountancyQueueName,
                    exchange: _ceoExchangeName,
                    routingKey: _accountancyReportRequestRoutingKey);

                Console.WriteLine($"[AccountancyService] Bound queue to traders exchange with routing key: {_accountancyDataRoutingKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error binding to traders exchange: {ex.Message}");
            }

            Console.WriteLine("[AccountancyService] Message queue initialization completed");
        }

        public async Task PublishMessageAsync(string exchange, string receivcer, string message)
        {
            ThrowIfDisposed();
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: receivcer,
                body: body);

            Console.WriteLine($"[AccountancyService] Published message");
        }

        public async Task PublishMessageAsync(string exchange, string receivcer, byte[] message)
        {
            ThrowIfDisposed();

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: receivcer,
                body: message);

            Console.WriteLine($"[AccountancyService] Published message");
        }

        public async Task PublishMessageAsync<T>(T obj)
        {
            ThrowIfDisposed();
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            await PublishMessageAsync(json);
        }

        public async Task<OperationResult> ReceiveMessageAsync()
        {
            ThrowIfDisposed();
            _cancellationTokenSource = new CancellationTokenSource();
            var completionSource = new TaskCompletionSource<OperationResult>();

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                Console.WriteLine($"[AccountancyService] Received message with routing key: {routingKey}");

                try
                {
                    if (routingKey == _accountancyReportRequestRoutingKey)
                    {
                        var result = await _accountancyService.ProcessReportCreatingRequestAsync(message);

                        if (result.Success)
                        {
                            Console.WriteLine($"[AccountancyService] Successfully processed with exit message: {result.Message}");

                            var dataByets = result.Data!.ToString();

                            await PublishMessageAsync(_accountancyExchangeName, _dmsReportRoutingKey , dataByets!);
                        }
                        else
                        {
                            Console.WriteLine($"[AccountancyService] Failed to process message: {result.Message}");
                        }
                    }

                    if (routingKey == _accountancyDataRoutingKey)
                    {
                        var result = await _accountancyService.ProcessTradersRequestAsync(message);

                        if (result.Success)
                        {
                            Console.WriteLine($"[AccountancyService] Successfully processed message: {result.Message}");
                        }
                        else
                        {
                            Console.WriteLine($"[AccountancyService] Failed to process message: {result.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[AccountancyService] Error processing message: {ex.Message}");
                }
            };

            string consumerTag = await _channel.BasicConsumeAsync(
                queue: _accountancyQueueName,
                autoAck: true,
                consumer: _consumer);

            Console.WriteLine($"[AccountancyService] Waiting for messages. Consumer tag: {consumerTag}");
            Console.WriteLine("[AccountancyService] Press Ctrl+C to exit.");

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
                    Console.WriteLine($"[AccountancyService] Error stopping consumer: {ex.Message}");
                }
            }
        }

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(AccountancyMessageBroker));
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