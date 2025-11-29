using FoxSky.StocksSystem.Accountancy.Database.Context;
using FoxSky.StocksSystem.Accountancy.Database.Entities;
using FoxSky.StocksSystem.Accountancy.Models;
using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.SharedServices.Models;
using FoxSky.StocksService.SharedServices.Models;
using Newtonsoft.Json;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

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

        public async Task<OperationResult> ProcessReportCreatingRequestAsync(string parameters)
        {
            QuestPDF.Settings.License = LicenseType.Community;
            DateTime endDate;
            List<Trade> tradeList = new();
            ReportModel reportModel;

            try
            {
                // Deserialize parameters into ReportRequestModel
                var requestData = JsonConvert.DeserializeObject<ReportRequestModel>(parameters);

                if (requestData == null)
                {
                    return OperationResult.Failed(message: "Failed to deserialize report request data.");
                }

                DateTime beginningDate = requestData.BeginningDate;
                endDate = requestData.EndDate;

                // Retrieve trades data based on date range
                if (beginningDate.Date == endDate.Date)
                {
                    tradeList = await ReportDocumentDataSource.RetreiveTradesData(_context, beginningDate);
                }
                else
                {
                    tradeList = await ReportDocumentDataSource.RetreiveTradesData(_context, beginningDate, endDate);
                }

                reportModel = new ReportModel
                {
                    ReportId = requestData.ReportId,
                    Commissioner = new CommissionerDataModel
                    {
                        CommissionerId = requestData.CommissionerId
                    },
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

                // Save report metadata to database
                var report = new Report
                {
                    ReportId = requestData.ReportId,
                    CreatedAt = DateTime.UtcNow
                };

                await _context.Reports.AddAsync(report);
                await _context.SaveChangesAsync();

                Console.WriteLine($"[AccountancyService] Report metadata saved with ReportId: {report.ReportId}");

                return OperationResult.Succeeded($"Report genereated successfuly", data: dmsReport);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error creating report: {ex.Message}");
                return OperationResult.Failed($"Error creating report: {ex.Message}");
            }
        }

        public async Task<OperationResult> ReceiveReportNotificationAsync(string data)
        {
            Console.WriteLine($"[AccountancyService] Received report notification: {data}");
            try
            {
                var response = JsonConvert.DeserializeObject<ReportResponseModel>(data);
                
                if (response == null)
                {
                    return OperationResult.Failed("Failed to deserialize report response data.");
                }

                var report = await _context.Reports.FindAsync(response.ReportId);
                
                if (report != null)
                {
                    report.DmsDocumentId = response.DmsDocumentId;
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"[AccountancyService] Updated Report {response.ReportId} with DMS ID {response.DmsDocumentId}");
                    return OperationResult.Succeeded($"Report {response.ReportId} updated with DMS document ID.");
                }
                else
                {
                    Console.WriteLine($"[AccountancyService] Report {response.ReportId} not found.");
                    return OperationResult.Failed($"Report {response.ReportId} not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AccountancyService] Error processing report notification: {ex.Message}");
                return OperationResult.Failed($"Error processing report notification: {ex.Message}");
            }
        }
    }
}
