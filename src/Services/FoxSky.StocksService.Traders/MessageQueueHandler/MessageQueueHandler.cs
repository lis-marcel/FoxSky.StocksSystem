using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.Traders.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace FoxSky.StocksService.Traders.MessageQueueHandler
{
    internal class MessageQueueHandler : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly string _queueName;
        private readonly ITradersService _tradersService;
        private AsyncEventingBasicConsumer? _consumer;

        public MessageQueueHandler() 
        {
            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI");
            var clientName = Environment.GetEnvironmentVariable("MESSAGE_BROKER_CONSUMER_NAME");
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
            _tradersService = new TradersService();
        }
             
        public static async Task<MessageQueueHandler> CreateAsync()
        {
            var messageQueue = new MessageQueueHandler();
            await messageQueue.InitializeAsync();
            return messageQueue;
        }

        private async Task InitializeAsync()
        {
            await ConfigureMessageBrokerAsync();
        }

        private async Task ConfigureMessageBrokerAsync()
        {
            await _channel.QueueDeclareAsync(
                queue: _queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.ExchangeDeclareAsync(
                exchange: _exchangeName,
                type: ExchangeType.Direct,
                durable: false,
                autoDelete: false,
                arguments: null);

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);
        }

        public async Task<OperationResult> ReceiveMessageAsync()
        {
            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[StocksService] Received stock data: {message}");

                try
                {
                    var result = await _tradersService.ProcessStockDataAsync(message);

                    if (result.Success)
                    {
                        Console.WriteLine($"[StocksService] Successfully processed stock data: {result.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"[StocksService] Failed to process stock data: {result.Message}");
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[StocksService] Error processing message: {ex.Message}");
                }
            };

            string consumerTag = await _channel.BasicConsumeAsync(
                queue: _queueName,
                autoAck: false,
                consumer: _consumer);

            Console.WriteLine($"[StocksService] Waiting for messages. Consumer tag: {consumerTag}");
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
