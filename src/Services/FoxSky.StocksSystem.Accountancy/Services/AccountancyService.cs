using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Database.Context;
using FoxSky.StocksSystem.Accountancy.Database.Entities;
using FoxSky.StocksSystem.Accountancy.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class AccountancyService : IAccountancyService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly AccountancyDbContext _context;

        public AccountancyService(IServiceProvider serviceProvider, AccountancyDbContext dbContext)
        {
            _serviceProvider = serviceProvider /*?? throw new ArgumentNullException(nameof(serviceProvider))*/;
            _context = dbContext /*?? throw new ArgumentNullException(nameof(dbContext))*/;
        }
        
        public async Task<OperationResult> ProcessTradersRequestAsync(string data)
        {
            try
            {
                var requestData = JsonConvert.DeserializeObject<AccountancyRequestModel>(data);

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

        public async Task<OperationResult> ProcessTradersRequestAsync(AccountancyRequestModel requestData)
        {
            try
            {
                // Create a new Trade entity
                var trade = new Trade
                {
                    Ticker = requestData.Ticker,
                    Price = requestData.Price,
                    Quantity = requestData.Quantity,
                    TradingAction = requestData.TradingAction,
                    DecisionTime = requestData.DecisionTime
                };

                // Add to database
                await _context.Trades.AddAsync(trade);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[AccountancyService] Trade saved to database with ID: {trade.Id}");


                return OperationResult.Succeeded($"Trade processed and saved with ID: {trade.Id}", requestData);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error saving trade to database: {ex.Message}");
                return OperationResult.Failed($"Error saving trade to database: {ex.Message}");
            }
        }

        public async Task<OperationResult> ProcessReportCreatingRequestAsync(string range)
        {
            try
            {
                var datesRange = range.Split(",");

                if (datesRange.Length != 2)
                {
                    return OperationResult.Failed("Invalid date range format. Expected format: 'YYYY-MM-DD,YYYY-MM-DD'");
                }

                var beginningDate = DateTime.Parse(datesRange[0]);
                var endDate = DateTime.Parse(datesRange[1]);

                var data = await ReportDocumentDataSource.RetreiveTradesData(_context, beginningDate, endDate);

                return OperationResult.Succeeded($"Report genereated successfuly");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error saving trade to database: {ex.Message}");
                return OperationResult.Failed($"Error saving trade to database: {ex.Message}");
            }
        }
    }
}
