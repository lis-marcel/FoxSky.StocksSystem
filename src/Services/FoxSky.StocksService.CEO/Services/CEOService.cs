using FoxSky.StocksService.Ceo.Database.Context;
using FoxSky.StocksService.CEO.MessageBroker;
using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksService.CEO.Services
{
    public class CeoService : ICeoService
    {
        private readonly ICEOMessageBroker _serviceProvider;
        private readonly CeoDbContext _dbContext;

        public CeoService(ICEOMessageBroker serviceProvider, CeoDbContext dbContext)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public async Task<OperationResult> RequestReportGeneration()
        {
            // Example date range
            string datesRange = "2025-01-05 09:30:00,2025-01-12 15:05:00";
            string exchange = Environment.GetEnvironmentVariable("CEO_EXCHANGE_NAME")!;
            string routingKey = Environment.GetEnvironmentVariable("ACCOUNTANCY_REPORT_REQUEST_ROUTING_KEY")!;


            await _serviceProvider.PublishMessageAsync(
                exchange!,
                routingKey!,
                datesRange);

            Console.WriteLine("[CeoService] Requesting report generation...");
            return OperationResult.Succeeded("Report generation requested.");
        }

        public async Task<OperationResult> DownloadReport(string reportId)
        {
            Console.WriteLine($"[CeoService] Downloading report ID: {reportId}");
            return OperationResult.Succeeded($"Report {reportId} downloaded.");
        }

        public async Task<OperationResult> ReceiveReportData(string data)
        {
            Console.WriteLine($"[CeoService] Received report ID: {data}");
            return OperationResult.Succeeded($"Report {data} received.");
        }
    }
}
