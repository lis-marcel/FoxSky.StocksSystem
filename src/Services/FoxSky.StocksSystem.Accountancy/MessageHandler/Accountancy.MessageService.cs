using FoxSky.StocksService.SharedServices;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using FoxSky.StocksSystem.Accountancy.Services;

namespace FoxSky.StocksSystem.Accountancy.MessageQueueHandler
{
    internal class MessageQueueHandler : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _stocksProviderExchangeName;
        private readonly string _accountancyExchangeName;
        private readonly string _ceoDataRoutingKey;
        private readonly string _accountancyQueueName;
        private readonly IAccountancyService _accountancyService;
        private AsyncEventingBasicConsumer? _consumer;

        public MessageQueueHandler()
        {
            var messageUri = Environment.GetEnvironmentVariable("MESSAGE_BROKER_URI");
            _accountancyExchangeName = Environment.GetEnvironmentVariable("ACCOUNTANCY_EXCHANGE_NAME")!;
            _ceoDataRoutingKey = Environment.GetEnvironmentVariable("CEO_DATA_ROUTING_KEY")!;
            _accountancyQueueName = Environment.GetEnvironmentVariable("ACCOUNTANCY_QUEUE_NAME")!;

            if (string.IsNullOrEmpty(messageUri) || string.IsNullOrEmpty(_accountancyExchangeName) ||
                string.IsNullOrEmpty(_stocksProviderExchangeName) || string.IsNullOrEmpty(_ceoDataRoutingKey) ||
                string.IsNullOrEmpty(_accountancyQueueName))
                throw new ArgumentNullException("Message broker configuration is not set in environment variables.");

            var factory = new ConnectionFactory()
            {
                Uri = new Uri(messageUri),
            };

            _connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            _channel = _connection.CreateChannelAsync().GetAwaiter().GetResult();
            _accountancyService = new AccountancyService();
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
                queue: _accountancyQueueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            await _channel.ExchangeDeclareAsync(
                exchange: _accountancyExchangeName,
                type: ExchangeType.Topic,
                durable: false,
                autoDelete: false,
                arguments: null);

            await _channel.QueueBindAsync(
                queue: _accountancyQueueName,
                exchange: _stocksProviderExchangeName,
                routingKey: _ceoDataRoutingKey
                );
        }

        public async Task PublishMessageAsync(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);

            await _channel.BasicPublishAsync(
                exchange: _accountancyExchangeName,
                routingKey: _ceoDataRoutingKey,
                body: body);

            Console.WriteLine($"[AccountancyService] Published message.");
            Console.ReadLine();
        }

        public async Task PublishMessageAsync<T>(T obj)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(obj);
            var body = Encoding.UTF8.GetBytes(json);

            await _channel.BasicPublishAsync(
                exchange: _accountancyExchangeName,
                routingKey: _ceoDataRoutingKey,
                body: body);

            Console.WriteLine($"[AccountancyService] Published message.");
            Console.ReadLine();
        }

        public async Task<OperationResult> ReceiveMessageAsync()
        {
            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"[AccountancyService] Received stock data.");

                try
                {
                    var result = await _accountancyService.ProcessTradersRequestAsync(message);

                    if (result.Success)
                    {
                        Console.WriteLine($"[AccountancyService] Successfully processed stock data: {result.Message}");
                    }
                    else
                    {
                        Console.WriteLine($"[AccountancyService] Failed to process stock data: {result.Message}");
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
