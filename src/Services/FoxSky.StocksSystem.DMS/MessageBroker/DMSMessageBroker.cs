using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.SharedServices.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text.Json;

namespace FoxSky.StocksSystem.DMS.MessageBroker
{
    public class DMSMessageBroker : IDMSMessageBroker, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string messageUri;
        private readonly string _accountancyExchangeName;
        private readonly string _dmsExchangeName;
        private readonly string _dmsQueueName;
        private readonly string _accountancyReportDocumentRoutingKey;
        private readonly string _dmsReportRoutingKey;
        private AsyncEventingBasicConsumer? _consumer;
        private bool _disposed;
        private CancellationTokenSource? _cancellationTokenSource;

        public DMSMessageBroker()
        {
            messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI") 
                ?? throw new InvalidOperationException("MESSAGE_BROKER_URI value not set");

            _accountancyExchangeName = Environment.GetEnvironmentVariable("ACCOUNTANCY_EXCHANGE_NAME")
                ?? throw new InvalidOperationException("ACCOUNTANCY_EXCHANGE_NAME value not set");

            _dmsExchangeName = Environment.GetEnvironmentVariable("DMS_EXCHANGE_NAME") 
                ?? throw new InvalidOperationException("DMS_EXCHANGE_NAME value not set");

            _accountancyReportDocumentRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_REPORT_DOCUMENT_ROUTING_KEY") 
                ?? throw new InvalidOperationException("ACCOUNTANCY_REPORT_DOCUMENT_ROUTING_KEY value not set");

            _dmsQueueName = Environment.GetEnvironmentVariable("DMS_QUEUE_NAME") 
                ?? throw new InvalidOperationException("DMS_QUEUE_NAME value not set");

            _dmsReportRoutingKey = Environment.GetEnvironmentVariable("DMS_REPORT_ROUTING_KEY")
                ?? throw new InvalidOperationException("DMS_REPORT_ROUTING_KEY value not set");

            var factory = new ConnectionFactory { Uri = new Uri(messageUri) };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
        }

        public async Task InitializeAsync()
        {
            Console.WriteLine("[DMS] Initializing message queue and exchange...");

            await _channel.QueueDeclareAsync(
                queue: _dmsQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[DMS] Declared queue: {_dmsQueueName}");

            await _channel.ExchangeDeclareAsync(
                exchange: _dmsExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

            Console.WriteLine($"[DMS] Declared exchange: {_dmsExchangeName}");

            try
            {
                await _channel.QueueBindAsync(
                    queue: _dmsQueueName,
                    exchange: _accountancyExchangeName,
                    routingKey: _dmsReportRoutingKey);

                Console.WriteLine($"[DMS] Bound queue to traders exchange with routing key: {_dmsReportRoutingKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DMS] Error binding to traders exchange: {ex.Message}");
            }
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
                var routingKey = ea.RoutingKey;

                Console.WriteLine($"[DMS] Received message with routing key: {routingKey}");

                try
                {
                    if (routingKey == _dmsReportRoutingKey)
                    {
                        Console.WriteLine($"[DMS] received data.");
                        var reportModel = JsonSerializer.Deserialize<DmsReportModel>(body);

                        // To-Do: Implement logic to save to MongoDB
                        // await _mongoDbService.SaveDocumentAsync(reportModel);
                        Console.WriteLine($"[DMS] Stored report {reportModel!.ReportId} in the database.");

                        // To-Do: Implement notification logic
                        // await _notificationService.NotifyCommissionerAsync(reportModel.CommisionerId, reportModel.ReportId);
                        Console.WriteLine($"[DMS] Sent notification to {reportModel.CommissionerEmail}.");
                    }
                    else
                    {
                        Console.WriteLine("[DMS] Something went wrong during data TX/RX");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[DMS] Error processing message: {ex.Message}");
                }
            };

            string consumerTag = await _channel.BasicConsumeAsync(
                queue: _dmsQueueName,
                autoAck: true,
                consumer: _consumer);

            Console.WriteLine($"[DMS] Waiting for messages. Consumer tag: {consumerTag}");
            Console.WriteLine("[DMS] Press Ctrl+C to exit.");

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
                    Console.WriteLine($"[DMS] Error stopping consumer: {ex.Message}");
                }
            }
        }
            
        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(DMSMessageBroker));
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
