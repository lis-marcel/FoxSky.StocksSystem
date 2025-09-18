using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.Traders.BOs;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class AccountancyService : IAccountancyService
    {
        public async Task<OperationResult> ProcessTradersRequestAsync(string data)
        {
            Thread.Sleep(500); // Simulate processing delay

            Console.WriteLine($"[AccountancyService] Processing traders request with data: {data}");

            return OperationResult.Succeeded();
        }

        public async Task<OperationResult> ProcessTradersRequestAsync(StockData data)
        {
            Thread.Sleep(500); // Simulate processing delay

            Console.WriteLine($"[AccountancyService] Processing traders request with data: {data}");

            return OperationResult.Succeeded();
        }
    }
}
