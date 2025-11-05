using FoxSky.StocksService.SharedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
