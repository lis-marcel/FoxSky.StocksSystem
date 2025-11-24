using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksService.CEO.MessageBroker
{
    public interface ICEOMessageBroker
    {
        Task InitializeAsync();
        Task<OperationResult> ReceiveMessageAsync();
        Task PublishMessageAsync(string exchange, string receivcer, string message);
        Task PublishMessageAsync(string exchange, string receivcer, byte[] message);
        Task StopReceivingAsync();
    }
}
