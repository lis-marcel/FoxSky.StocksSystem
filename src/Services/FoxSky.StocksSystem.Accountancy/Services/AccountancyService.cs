using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.Traders.BOs;
using FoxSky.StocksSystem.Accountancy.Models;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class AccountancyService : IAccountancyService
    {
        public async Task<OperationResult> ProcessTradersRequestAsync(string data)
        {
            Thread.Sleep(500); // Simulate processing delay

            var stockData = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountancyRequest>(data);

            if (stockData == null)
            {
                return OperationResult.Failed("Failed to deserialize stock data.");
            }

            Console.WriteLine($"[AccountancyService] Processing traders request with data: {stockData.Ticker}, {stockData.Price}, {stockData.Quantity}");

            return OperationResult.Succeeded(data: stockData);
        }

        public async Task<OperationResult> ProcessTradersRequestAsync(AccountancyRequest data)
        {
            Thread.Sleep(500); // Simulate processing delay

            Console.WriteLine($"[AccountancyService] Processing traders request with data: {data}");

            return OperationResult.Succeeded();
        }
    }
}
