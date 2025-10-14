using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Models;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public interface IAccountancyService
    {
        Task<OperationResult> ProcessTradersRequestAsync(string data);
        Task<OperationResult> ProcessTradersRequestAsync(AccountancyRequest data);
        Task<OperationResult> ProcessReportCreatingRequestAsync(string message);
    }
}
