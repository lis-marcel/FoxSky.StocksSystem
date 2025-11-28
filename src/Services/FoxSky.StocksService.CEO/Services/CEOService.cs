using FoxSky.StocksService.Ceo.Database.Context;
using FoxSky.StocksService.CEO.MessageBroker;
using FoxSky.StocksService.SharedServices.Models;
using FoxSky.StocksSystem.SharedServices;
using System.Runtime.InteropServices;

namespace FoxSky.StocksService.CEO.Services
{
    public class CeoService : ICeoService
    {
        private readonly ICeoMessageBroker _serviceProvider;
        private readonly CeoDbContext _dbContext;

        public CeoService(ICeoMessageBroker serviceProvider, CeoDbContext dbContext)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult> RequestReportGeneration()
        {
            // Example data
            var reportRequestModel = new ReportRequestModel
            {
                ReportId = Guid.NewGuid(),
                CommissionerId = 1,
                BeginningDate = new DateTime(2025, 1, 5, 9, 30, 0),
                EndDate = new DateTime(2025, 1, 12, 15, 5, 0),
            };

            string serializedModel = Newtonsoft.Json.JsonConvert.SerializeObject(reportRequestModel);

            string ceoExchange = Environment.GetEnvironmentVariable("CEO_EXCHANGE_NAME")!;
            string accountancyRoutingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY")!;

            await _serviceProvider.PublishMessageAsync(
                ceoExchange!,
                accountancyRoutingKey!,
                serializedModel);

            Console.WriteLine("[CeoService] Requesting report generation...");
            return OperationResult.Succeeded("Report generation requested.");
        }
                
        public async Task<OperationResult> ReceiveReportData(string data)
        {
            Console.WriteLine($"[CeoService] Received report ID: {data}");
            return OperationResult.Succeeded($"Report {data} received.");
        }

        public async Task<OperationResult> DownloadReport(string reportId)
        {
            Console.WriteLine($"[CeoService] Downloading report ID: {reportId}");
            return OperationResult.Succeeded($"Report {reportId} downloaded.");
        }
    }
}
