using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksSystem.DMS.MessageBroker
{
    public interface IDmsMessageBroker
    {
        Task InitializeAsync();
        Task<OperationResult> ReceiveMessageAsync();
        Task PublishMessageAsync(string exchange, string receiver, string message);
        Task PublishMessageAsync(string exchange, string receiver, byte[] message);
        Task StopReceivingAsync();
    }
}
