using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksSystem.DMS.MessageBroker
{
    public interface IDMSMessageBroker
    {
        Task InitializeAsync();
        Task<OperationResult> ReceiveMessageAsync();
        //Task PublishMessageAsync(string message);
        //Task PublishMessageAsync<T>(T obj);
        Task StopReceivingAsync();
    }
}
