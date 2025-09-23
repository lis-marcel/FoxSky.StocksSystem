using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Models;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class AccountancyService : IAccountancyService
    {
        public async Task<OperationResult> ProcessTradersRequestAsync(string data)
        {
            Thread.Sleep(500); // Simulate processing delay

            var requestData = Newtonsoft.Json.JsonConvert.DeserializeObject<AccountancyRequest>(data);

            if (requestData == null)
            {
                return OperationResult.Failed("Failed to deserialize stock data.");
            }

            Console.WriteLine($"[AccountancyService] Processing traders request with data: " +
                $"{requestData.Ticker}, " +
                $"{requestData.Price}, " +
                $"{requestData.Quantity} " +
                $"{requestData.TradingAction}, " +
                $"{requestData.DecisionTime}");

            return OperationResult.Succeeded(data: requestData);
        }

        public async Task<OperationResult> ProcessTradersRequestAsync(AccountancyRequest requestData)
        {
            Thread.Sleep(500); // Simulate processing delay

            Console.WriteLine($"[AccountancyService] Processing traders request with data: " +
                $"{requestData.Ticker}, " +
                $"{requestData.Price}, " +
                $"{requestData.Quantity} " +
                $"{requestData.TradingAction}, " +
                $"{requestData.DecisionTime}");

            return OperationResult.Succeeded(data: requestData);
        }
    }
}
