using FoxSky.StocksService.SharedServices;

namespace FoxSky.StocksSystem.Accountancy.MessageBroker
{
    public interface IAccountancyMessageBroker
    {
        Task InitializeAsync();
        Task<OperationResult> ReceiveMessageAsync();
        Task PublishMessageAsync(string message);
        Task PublishMessageAsync<T>(T obj);
        Task StopReceivingAsync();
    }
}