using FoxSky.StocksService.SharedServices;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FoxSky.StocksSystem.Accountancy.MessageBroker
{
    public interface IAccountancyMessageBroker
    {
        Task InitializeAsync();
        Task<OperationResult> ReceiveMessageAsync();
        Task PublishMessageAsync(string exchange, string receivcer, string message);
        Task PublishMessageAsync(string exchange, string receivcer, byte[] message);
        Task PublishMessageAsync<T>(T obj);
        Task StopReceivingAsync();
    }
}