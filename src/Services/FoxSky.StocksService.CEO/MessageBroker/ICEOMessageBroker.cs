namespace FoxSky.StocksService.CEO.MessageBroker
{
    public interface ICEOMessageBroker
    {
        Task InitializeAsync();
        //Task<OperationResult> ReceiveMessageAsync();
        Task PublishMessageAsync(string message);
        //Task PublishMessageAsync<T>(T obj);
        Task StopReceivingAsync();
    }
}
