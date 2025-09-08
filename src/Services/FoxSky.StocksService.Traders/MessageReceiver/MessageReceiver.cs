using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace FoxSky.StocksService.Traders.MessageReceiver
{
    internal class MessageReceiver : IDisposable
    {
        private readonly IConnection _connection;
        private readonly IChannel _channel;
        private readonly string _exchangeName;
        private readonly string _routingKey;
        private readonly string _queueName;
        private AsyncEventingBasicConsumer? _consumer;

        public MessageReceiver() 
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
        }
             
        public static async Task<MessageReceiver> CreateAsync()
        {
            var messageQueue = new MessageReceiver();
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

            await _channel.BasicQosAsync(
                prefetchSize: 0,
                prefetchCount: 1,
                global: false);
        }

        public async Task ReceiveMessageAsync()
        {
            _consumer = new AsyncEventingBasicConsumer(_channel);

            _consumer.ReceivedAsync += async (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine($"[StocksService] Received stock data: {message}");
                // Simulate processing time
                await Task.Delay(500);
                // Acknowledge the message
                await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
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
        }

        public void Dispose()
        {
            _channel?.Dispose();
            _connection?.Dispose();
        }
    }
}
