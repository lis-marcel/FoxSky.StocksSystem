using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Traders.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FoxSky.StocksService.Traders.MessageQueueHandler
{
    internal class TradersMessageBroker : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _stocksProviderExchangeName;
        private readonly string _tradersExchangeName;
        private readonly string _stocksDataRoutingKey;
        private readonly string _accountancyDataRoutingKey;
        private readonly string _tradersQueueName;
        private readonly ITradersService _tradersService;
        private AsyncEventingBasicConsumer? _consumer;

        public TradersMessageBroker() 
        {
            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI");
            _tradersExchangeName = Environment.GetEnvironmentVariable("TRADERS_EXCHANGE_NAME")!;
            _stocksProviderExchangeName = Environment.GetEnvironmentVariable("STOCKS_PROVIDER_EXCHANGE_NAME")!;
            _stocksDataRoutingKey = Environment.GetEnvironmentVariable("STOCKS_DATA_ROUTING_KEY")!;
            _accountancyDataRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_DATA_ROUTING_KEY")!;
            _tradersQueueName = Environment.GetEnvironmentVariable("TRADERS_QUEUE_NAME")!;

            if (string.IsNullOrEmpty(messageUri) || string.IsNullOrEmpty(_tradersExchangeName) ||
                string.IsNullOrEmpty(_stocksProviderExchangeName) || string.IsNullOrEmpty(_stocksDataRoutingKey) ||
                string.IsNullOrEmpty(_accountancyDataRoutingKey) || string.IsNullOrEmpty(_tradersQueueName))
                throw new ArgumentNullException("Message broker configuration is not set in environment variables.");

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(messageUri),
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _tradersService = new TradersService();
        }
             
        public static async Task<TradersMessageBroker> CreateAsync()
        {
            var messageQueue = new TradersMessageBroker();
            await messageQueue.InitializeAsync();
            return messageQueue;
        }

        private async Task InitializeAsync()
        {
            Console.WriteLine("[TradersService] Initializing message queues and exchanges...");
            
            await _channel.QueueDeclareAsync(
                queue: _tradersQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
                
            Console.WriteLine($"[TradersService] Declared queue: {_tradersQueueName}");

            await _channel.ExchangeDeclareAsync(
                exchange: _tradersExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);
                
            Console.WriteLine($"[TradersService] Declared exchange: {_tradersExchangeName}");

            await _channel.QueueBindAsync(
                queue: _tradersQueueName,
                exchange: _stocksProviderExchangeName,
                routingKey: _stocksDataRoutingKey);
                
            Console.WriteLine($"[TradersService] Bound queue to stocks provider exchange with routing key: {_stocksDataRoutingKey}");
                
            Console.WriteLine("[TradersService] Message queue initialization completed");
        }

        public async Task PublishMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: _tradersExchangeName,
                routingKey: _accountancyDataRoutingKey,
                body: body);

            Console.WriteLine($"[TradersService] Published message to exchange '{_tradersExchangeName}' with routing key '{_accountancyDataRoutingKey}'");
        }

        public async Task PublishMessageAsync<T>(T obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: _tradersExchangeName,
                routingKey: _accountancyDataRoutingKey,
                body: body);

            Console.WriteLine($"[TradersService] Published message to exchange '{_tradersExchangeName}' with routing key '{_accountancyDataRoutingKey}'");
        }

        public async Task<OperationResult> ReceiveMessageAsync()
        {
            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var routingKey = ea.RoutingKey;

                Console.WriteLine($"[TradersService] Received stock data with routing key: {routingKey}");

                try
                {
                    var result = await _tradersService.ProcessStockDataAsync(message);

                    if (result.Success)
                    {
                        Console.WriteLine($"[TradersService] Successfully processed stock data: {result.Message}");
                        
                        // After successful processing, publish the result to Accountancy
                        if (result.Data != null)
                        {
                            await PublishMessageAsync(result.Data.ToString()!);
                        }
                    }
                    else
                    {
                        Console.WriteLine($"[TradersService] Failed to process stock data: {result.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[TradersService] Error processing message: {ex.Message}");
                }
            };

            string consumerTag = await _channel.BasicConsumeAsync(
                queue: _tradersQueueName,
                autoAck: true,
                consumer: _consumer);

            Console.WriteLine($"[TradersService] Waiting for messages. Consumer tag: {consumerTag}");
            Console.WriteLine("[TradersService] Press [enter] to exit.");
            Console.ReadLine();

            await _channel.BasicCancelAsync(consumerTag);
            await _channel.CloseAsync();
            await _connection.CloseAsync();

            return OperationResult.Succeeded("Message processing completed");
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
