using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Models;
using FoxSky.StocksSystem.Accountancy.Database.Context;
using FoxSky.StocksSystem.Accountancy.Database.Entities;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class AccountancyService : IAccountancyService
    {
        private readonly IServiceProvider _serviceProvider;

        public AccountancyService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public async Task<OperationResult> ProcessTradersRequestAsync(string data)
        {
            try
            {
                var requestData = JsonConvert.DeserializeObject<AccountancyRequest>(data);

                if (requestData == null)
                {
                    return OperationResult.Failed("Failed to deserialize stock data.");
                }

                return await ProcessTradersRequestAsync(requestData);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Error processing request: {ex.Message}");
            }
        }

        public async Task<OperationResult> ProcessTradersRequestAsync(AccountancyRequest requestData)
        {
            try
            {
                Console.WriteLine($"[AccountancyService] Processing traders request with data: " +
                    $"{requestData.Ticker}, " +
                    $"{requestData.Price}, " +
                    $"{requestData.Quantity}, " +
                    $"{requestData.TradingAction}, " +
                    $"{requestData.DecisionTime}");

                // Create a new Trade entity
                var trade = new Trade
                {
                    Ticker = requestData.Ticker,
                    Price = requestData.Price,
                    Quantity = requestData.Quantity,
                    TradingAction = requestData.TradingAction,
                    DecisionTime = requestData.DecisionTime
                };

                // Create a scope for database operations
                using (var scope = _serviceProvider.CreateScope())
                {
                    // Get a new DbContext instance for this operation
                    var dbContext = scope.ServiceProvider.GetRequiredService<AccountancyDbContext>();
                    
                    // Add to database
                    await dbContext.Trades.AddAsync(trade);
                    await dbContext.SaveChangesAsync();
                    
                    Console.WriteLine($"[AccountancyService] Trade saved to database with ID: {trade.Id}");
                }

                return OperationResult.Succeeded($"Trade processed and saved with ID: {trade.Id}", requestData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error saving trade to database: {ex.Message}");
                return OperationResult.Failed($"Error saving trade to database: {ex.Message}");
            }
        }
    }
}
