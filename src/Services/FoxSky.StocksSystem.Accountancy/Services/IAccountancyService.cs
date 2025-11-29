using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.Accountancy.Models;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public interface IAccountancyService
    {
        Task<OperationResult> ProcessTradersRequestAsync(string data);
        Task<OperationResult> ProcessTradersRequestAsync(AccountancyRequestModel data);
        Task<OperationResult> ProcessReportCreatingRequestAsync(string message);
        Task<OperationResult> ReceiveReportNotificationAsync(string data);
    }
}
