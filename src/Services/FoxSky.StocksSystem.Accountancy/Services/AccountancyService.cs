using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.SharedServices.Models;
using FoxSky.StocksSystem.Accountancy.Database.Context;
using FoxSky.StocksSystem.Accountancy.Database.Entities;
using FoxSky.StocksSystem.Accountancy.Models;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;
using System.Reflection;

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
            QuestPDF.Settings.License = LicenseType.Community;
            DateTime endDate;
            List<Trade> tradeList = new();
            ReportModel reportModel;

            try
            {
                if (string.IsNullOrEmpty(range))
                {
                    return OperationResult.Failed(message: "At least one date is required for report.");
                }
                
                var datesRange = range.Split(",", StringSplitOptions.RemoveEmptyEntries)
                    .Select(r => r.Trim()
                    .ToArray());

                if (!DateTime.TryParse(datesRange.ElementAtOrDefault(0), out DateTime beginningDate))
                {
                    return OperationResult.Failed(message: "Invalid beginning date.");
                }

                if (datesRange.Count() == 1)
                {
                    endDate = beginningDate;
                    tradeList = await ReportDocumentDataSource.RetreiveTradesData(_context, beginningDate);
                }
                else
                {
                    if (!DateTime.TryParse(datesRange.ElementAtOrDefault(1), out endDate)) 
                    {
                        return OperationResult.Failed(message: "Invalid end date.");
                    }

                    tradeList = await ReportDocumentDataSource.RetreiveTradesData(_context, beginningDate, endDate);
                }

                reportModel = new ReportModel
                {
                    ReportId = Guid.NewGuid(),
                    Commissioner = new CommissionerDataModel(),
                    Issuer = new IssuerDataModel(),
                    BeginningDate = beginningDate,
                    EndDate = endDate,
                    IssueDate = DateTime.Now,
                    Trades = tradeList
                };

                var document = new ReportDocumentService(reportModel);
                var documentBytes = document.GeneratePdf();

                var dmsReport = new DmsReportModel
                {
                    ReportId = reportModel.ReportId,
                    Document = documentBytes,
                    IssuerId = reportModel.Issuer.IssuerId,
                    CommissionerId = reportModel.Commissioner.CommissionerId,
                    CommissionerEmail = reportModel.Commissioner.CommissionerEmail,
                    IssueDate = reportModel.IssueDate
                };

                return OperationResult.Succeeded($"Report genereated successfuly", data: dmsReport);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error saving trade to database: {ex.Message}");
                return OperationResult.Failed($"Error saving trade to database: {ex.Message}");
            }
        }
    }
}
