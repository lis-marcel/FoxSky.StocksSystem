using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.Traders.BOs;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    internal interface IAccountancyService
    {
        Task<OperationResult> ProcessTradersRequestAsync(string data);
        Task<OperationResult> ProcessTradersRequestAsync(StockData data);
    }
}
