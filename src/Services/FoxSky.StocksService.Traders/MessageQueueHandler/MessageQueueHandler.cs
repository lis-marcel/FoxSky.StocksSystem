using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.Traders.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace FoxSky.StocksService.Traders.MessageQueueHandler
{
    internal class MessageQueueHandler : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _stocksProviderExchangeName;
        private readonly string _tradersExchangeName;
        private readonly string _stocksDataRoutingKey;
        private readonly string _tradersQueueName;
        private readonly ITradersService _tradersService;
        private AsyncEventingBasicConsumer? _consumer;

        public MessageQueueHandler() 
        {
            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI");
            _tradersExchangeName = Environment.GetEnvironmentVariable("TRADERS_EXCHANGE_NAME")!;
            _stocksProviderExchangeName = Environment.GetEnvironmentVariable("STOCKS_PROVIDER_EXCHANGE_NAME")!;
            _stocksDataRoutingKey = Environment.GetEnvironmentVariable("STOCKS_DATA_ROUTING_KEY")!;
            _tradersQueueName = Environment.GetEnvironmentVariable("TRADERS_QUEUE_NAME")!;

            if (string.IsNullOrEmpty(messageUri) || string.IsNullOrEmpty(_tradersExchangeName) ||
                string.IsNullOrEmpty(_stocksProviderExchangeName) || string.IsNullOrEmpty(_stocksDataRoutingKey) ||
                string.IsNullOrEmpty(_tradersQueueName))
                throw new ArgumentNullException("Message broker configuration is not set in environment variables.");

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(messageUri),
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _tradersService = new TradersService();
        }
             
        public async Task<MessageQueueHandler> CreateAsync()
        {
            var messageQueue = new MessageQueueHandler();
            await messageQueue.InitializeAsync();
            return messageQueue;
        }

        private async Task InitializeAsync()
        {
            await _channel.QueueDeclareAsync(
                queue: _tradersQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.ExchangeDeclareAsync(
                exchange: _tradersExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueBindAsync(
                queue: _tradersQueueName,
                exchange: _stocksProviderExchangeName,
                routingKey: _stocksDataRoutingKey
                );
        }

        public async Task PublishMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: _tradersExchangeName,
                routingKey: _stocksDataRoutingKey,
                body: body);

            Console.WriteLine($"[TradersService] Published message.");
            Console.ReadLine();
        }

        public async Task PublishMessageAsync<T>(T obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: _tradersExchangeName,
                routingKey: _stocksDataRoutingKey,
                body: body);

            Console.WriteLine($"[TradersService] Published message.");
            Console.ReadLine();
        }

        public async Task<OperationResult> ReceiveMessageAsync()
        {
            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[TradersService] Received stock data.");

                try
                {
                    var result = await _tradersService.ProcessStockDataAsync(message);

                    if (result.Success)
                    {
                        Console.WriteLine($"[TradersService] Successfully processed stock data: {result.Message}");
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
            Console.WriteLine("Press [enter] to exit.");
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
