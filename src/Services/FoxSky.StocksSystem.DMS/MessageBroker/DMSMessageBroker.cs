using FoxSky.StocksSystem.DMS.Services;
using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.SharedServices.Models;
using FoxSky.StocksService.SharedServices.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json;
using MongoDB.Bson;

namespace FoxSky.StocksSystem.DMS.MessageBroker
{
    public class DmsMessageBroker : IDmsMessageBroker, IDisposable
    {
        #region Fields
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string messageUri;
        private readonly string _accountancyExchangeName;
        private readonly string _dmsExchangeName;
        private readonly string _dmsQueueName;
        private readonly string _accountancyReportDocumentRoutingKey;
        private readonly string _dmsReportRoutingKey;
        private readonly string _dmsReportNotificationRoutingKey;
        private readonly string _ceoReportDownloadRoutingKey;
        private AsyncEventingBasicConsumer? _consumer;
        private bool _disposed;
        private CancellationTokenSource? _cancellationTokenSource;
        private readonly IDmsService _dmsService;
        #endregion

        public DmsMessageBroker(IDmsService dmsService)
        {
            _dmsService = dmsService ?? throw new ArgumentNullException(nameof(dmsService)); ;

            #region Server Configuration
            messageUri = AppContext.GetData(name: "MessageBrokerConfig:ServerConfig:MessageBrokerURI") as string 
                ?? throw new InvalidOperationException("MessageBrokerURI value not set");
            #endregion
            #region Exchanges
            _accountancyExchangeName = AppContext.GetData(name: "MessageBrokerConfig:ExchangesConfig:AccountancyExchangeName") as string 
                ?? throw new InvalidOperationException("AccountancyExchangeName value not set");

            _dmsExchangeName = AppContext.GetData(name: "MessageBrokerConfig:ExchangesConfig:DmsExchangeName") as string 
                ?? throw new InvalidOperationException("DmsExchangeName value not set");
            #endregion
            #region Queues
            _dmsQueueName = AppContext.GetData(name: "MessageBrokerConfig:QueuesConfig:DmsQueueName") as string 
                ?? throw new InvalidOperationException("DmsQueueName value not set");
            #endregion
            #region Routing Keys
            _dmsReportRoutingKey = AppContext.GetData(name: "MessageBrokerConfig:RoutingKeysConfig:DmsReportSaveRoutingKey") as string 
                ?? throw new InvalidOperationException("DmsReportSaveRoutingKey value not set");

            _dmsReportNotificationRoutingKey = AppContext.GetData(name: "MessageBrokerConfig:RoutingKeysConfig:ReportNotificationRoutingKey") as string 
                ?? throw new InvalidOperationException("ReportNotificationRoutingKey value not set");

            _accountancyReportDocumentRoutingKey = AppContext.GetData(name: "MessageBrokerConfig:RoutingKeysConfig:AccountancyRoutingKey") as string
                ?? throw new InvalidOperationException("AccountancyRoutingKey value not set");

            _ceoReportDownloadRoutingKey = AppContext.GetData(name: "MessageBrokerConfig:RoutingKeysConfig:ReportDownloadRoutingKey") as string
                ?? throw new InvalidOperationException("ReportDownloadRoutingKey value not set");
            #endregion
            #region Create Connection
            var factory = new ConnectionFactory { Uri = new Uri(messageUri) };
            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            #endregion
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

            try
            {
                await _channel.QueueBindAsync(
                    queue: _dmsQueueName,
                    exchange: "ceo_data_exchange",
                    routingKey: _ceoReportDownloadRoutingKey);

                Console.WriteLine($"[DMS] Bound queue to CEO exchange with routing key: {_ceoReportDownloadRoutingKey}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[DMS] Error binding to CEO exchange: {ex.Message}");
            }
        }

        public async Task PublishMessageAsync(string exchange, string receivcer, string message)
        {
            ThrowIfDisposed();
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: receivcer,
                body: body);

            Console.WriteLine($"[DMS] Published message to {exchange}");
        }

        public async Task PublishMessageAsync(string exchange, string receivcer, byte[] message)
        {
            ThrowIfDisposed();

            await _channel.BasicPublishAsync(
                exchange: exchange,
                routingKey: receivcer,
                body: message);

            Console.WriteLine($"[DMS] Published message to {exchange}");
        }

        public async Task<OperationResult> ReceiveMessageAsync()
        {
            ThrowIfDisposed();
            _cancellationTokenSource = new CancellationTokenSource();
            var completionSource = new TaskCompletionSource<OperationResult>();

            _consumer = new AsyncEventingBasicConsumer(_channel);
            _consumer.ReceivedAsync += async (model, ea) => await Task.Run(async () =>
            {
                var body = ea.Body.ToArray();
                var routingKey = ea.RoutingKey;

                Console.WriteLine($"[DMS] Received message with routing key: {routingKey}");

                try
                {
                    if (routingKey == _dmsReportRoutingKey)
                    {
                        Console.WriteLine($"[DMS] received data.");
                        var saveResultData = await _dmsService.ProcessSaveDocumentRequestAsync(body);

                        if (saveResultData.Success)
                        {
                            Console.WriteLine($"[DMS] Saving operation suceeded: {saveResultData.Message}");

                            // Serialize ReportResponseModel to JSON
                            var responseModel = saveResultData.Data as ReportResponseModel;
                            if (responseModel != null)
                            {
                                var jsonResponse = JsonConvert.SerializeObject(responseModel);

                                PublishMessageAsync(
                                    exchange: _dmsExchangeName,
                                    receivcer: _dmsReportNotificationRoutingKey,
                                    message: jsonResponse).GetAwaiter().GetResult();

                                Console.WriteLine($"[DMS] Published report notification for ReportId: {responseModel.ReportId}");
                            }
                            else
                            {
                                Console.WriteLine($"[DMS] Failed to cast response data to ReportResponseModel");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[DMS] Saving operation failed: {saveResultData.Message}");
                        }
                    }
                    else if (routingKey == _ceoReportDownloadRoutingKey)
                    {
                        Console.WriteLine($"[DMS] Received document download request.");
                        var downloadRequest = JsonConvert.DeserializeObject<DocumentDownloadRequest>(Encoding.UTF8.GetString(body));
                        
                        if (downloadRequest != null && !string.IsNullOrEmpty(downloadRequest.DmsDocumentId))
                        {
                            try
                            {
                                var objectId = ObjectId.Parse(downloadRequest.DmsDocumentId);
                                var documentData = await _dmsService.GetDocumentAsync(objectId);
                                
                                if (documentData != null)
                                {
                                    var response = new DocumentDownloadResponse
                                    {
                                        ReportId = downloadRequest.ReportId,
                                        DocumentData = documentData,
                                        Success = true,
                                        Message = "Document retrieved successfully"
                                    };
                                    
                                    var jsonResponse = JsonConvert.SerializeObject(response);
                                    
                                    await PublishMessageAsync(
                                        exchange: _dmsExchangeName,
                                        receivcer: "ceo.report.document",
                                        message: jsonResponse);
                                    
                                    Console.WriteLine($"[DMS] Sent document for ReportId: {downloadRequest.ReportId}");
                                }
                                else
                                {
                                    Console.WriteLine($"[DMS] Document not found for ID: {downloadRequest.DmsDocumentId}");
                                    
                                    var response = new DocumentDownloadResponse
                                    {
                                        ReportId = downloadRequest.ReportId,
                                        Success = false,
                                        Message = "Document not found"
                                    };
                                    
                                    var jsonResponse = JsonConvert.SerializeObject(response);
                                    
                                    await PublishMessageAsync(
                                        exchange: _dmsExchangeName,
                                        receivcer: "ceo.report.document",
                                        message: jsonResponse);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"[DMS] Error retrieving document: {ex.Message}");
                            }
                        }
                        else
                        {
                            Console.WriteLine($"[DMS] Invalid download request");
                        }
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
            });

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
                throw new ObjectDisposedException(nameof(DmsMessageBroker));
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
