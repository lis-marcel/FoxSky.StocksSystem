using FoxSky.StocksService.Ceo.Database.Context;
using FoxSky.StocksService.Ceo.Models;
using FoxSky.StocksService.CEO.MessageBroker;
using FoxSky.StocksService.SharedServices.Models;
using FoxSky.StocksSystem.SharedServices;
using System.Runtime.InteropServices;

namespace FoxSky.StocksService.CEO.Services
{
    public class CeoService : ICeoService
    {
        private readonly CeoDbContext _dbContext;

        public CeoService(CeoDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult> RequestReportGeneration(ICeoMessageBroker messageBroker)
        {
            var reportId = Guid.NewGuid();
            // Example data
            var reportRequestModel = new ReportRequestModel
            {
                ReportId = reportId,
                CommissionerId = 1,
                BeginningDate = new DateTime(2025, 1, 5, 9, 30, 0),
                EndDate = new DateTime(2025, 1, 12, 15, 5, 0),
            };

            var report = new FoxSky.StocksService.Ceo.Database.Entities.Report
            {
                ReportId = reportId,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Reports.Add(report);
            await _dbContext.SaveChangesAsync();

            string serializedModel = Newtonsoft.Json.JsonConvert.SerializeObject(reportRequestModel);

            string ceoExchange = Environment.GetEnvironmentVariable("CEO_EXCHANGE_NAME")!;
            string accountancyRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY")!;

            await messageBroker.PublishMessageAsync(
                ceoExchange!,
                accountancyRoutingKey!,
                serializedModel);

            Console.WriteLine($"[CeoService] Requesting report generation for ReportId: {reportId}...");
            return OperationResult.Succeeded("Report generation requested.");
        }
                
        public async Task<OperationResult> ReceiveReportData(string data)
        {
            Console.WriteLine($"[CeoService] Received report data: {data}");
            try
            {
                var response = Newtonsoft.Json.JsonConvert.DeserializeObject<ReportResponseModel>(data);
                if (response != null)
                {
                    var report = await _dbContext.Reports.FindAsync(response.ReportId);
                    if (report != null)
                    {
                        report.DmsDocumentId = response.DmsDocumentId;
                        await _dbContext.SaveChangesAsync();
                        Console.WriteLine($"[CeoService] Updated Report {response.ReportId} with DMS ID {response.DmsDocumentId}");
                        return OperationResult.Succeeded($"Report {response.ReportId} updated.");
                    }
                    else
                    {
                        Console.WriteLine($"[CeoService] Report {response.ReportId} not found.");
                        return OperationResult.Failed($"Report {response.ReportId} not found.");
                    }
                }
                return OperationResult.Failed("Invalid response data.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CeoService] Error processing report data: {ex.Message}");
                return OperationResult.Failed(ex.Message);
            }
        }

        public async Task<OperationResult> DownloadReport(string reportId)
        {
            Console.WriteLine($"[CeoService] Downloading report ID: {reportId}");
            return OperationResult.Succeeded($"Report {reportId} downloaded.");
        }
    }
}
