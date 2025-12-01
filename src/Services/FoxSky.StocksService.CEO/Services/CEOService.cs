using FoxSky.StocksService.Ceo.Database.Context;
using FoxSky.StocksService.CEO.MessageBroker;
using FoxSky.StocksService.SharedServices.Models;
using FoxSky.StocksSystem.SharedServices;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace FoxSky.StocksService.CEO.Services
{
    public class CeoService : ICeoService
    {
        private readonly CeoDbContext _dbContext;
        private string? _latestReportId;

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
                var response = JsonConvert.DeserializeObject<ReportResponseModel>(data);
                if (response != null)
                {
                    var report = await _dbContext.Reports.FindAsync(response.ReportId);
                    if (report != null)
                    {
                        report.DmsDocumentId = response.DmsDocumentId;
                        await _dbContext.SaveChangesAsync();
                        _latestReportId = response.ReportId.ToString();
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

        public async Task<OperationResult> DownloadReport()
        {
            if (string.IsNullOrEmpty(_latestReportId))
            {
                Console.WriteLine($"[CeoService] No report ID available for download.");
                return OperationResult.Failed("No report ID available.");
            }

            var reportId = Guid.Parse(_latestReportId);
            return await DownloadReport(reportId.ToString());
        }

        public async Task<OperationResult> DownloadReport(string reportId)
        {
            try
            {
                if (!Guid.TryParse(reportId, out var reportGuid))
                {
                    return OperationResult.Failed("Invalid report ID format.");
                }

                var report = await _dbContext.Reports.FindAsync(reportGuid);
                if (report == null)
                {
                    Console.WriteLine($"[CeoService] Report {reportId} not found in database.");
                    return OperationResult.Failed($"Report {reportId} not found.");
                }

                if (string.IsNullOrEmpty(report.DmsDocumentId))
                {
                    Console.WriteLine($"[CeoService] Report {reportId} has no DMS document ID.");
                    return OperationResult.Failed($"Report {reportId} has no document.");
                }

                var downloadRequest = new DocumentDownloadRequest
                {
                    ReportId = reportGuid,
                    DmsDocumentId = report.DmsDocumentId
                };

                Console.WriteLine($"[CeoService] Requesting document download for ReportId: {reportId}, DmsDocumentId: {report.DmsDocumentId}");
                return OperationResult.Succeeded("Download request sent.", downloadRequest);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CeoService] Error requesting document download: {ex.Message}");
                return OperationResult.Failed(ex.Message);
            }
        }

        public async Task<OperationResult> SaveDocumentLocally(DocumentDownloadResponse response)
        {
            try
            {
                if (!response.Success || response.DocumentData == null || response.DocumentData.Length == 0)
                {
                    Console.WriteLine($"[CeoService] Invalid document response.");
                    return OperationResult.Failed("Invalid document data received.");
                }

                // Create downloads directory if it doesn't exist
                var downloadsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Downloads");
                if (!Directory.Exists(downloadsPath))
                {
                    Directory.CreateDirectory(downloadsPath);
                    Console.WriteLine($"[CeoService] Created downloads directory: {downloadsPath}");
                }

                // Save the document with ReportId as filename
                var fileName = $"Report_{response.ReportId}.pdf";
                var filePath = Path.Combine(downloadsPath, fileName);

                await File.WriteAllBytesAsync(filePath, response.DocumentData);
                
                Console.WriteLine($"[CeoService] Document saved successfully to: {filePath}");
                Console.WriteLine($"[CeoService] File size: {response.DocumentData.Length} bytes");
                
                return OperationResult.Succeeded($"Document saved to {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CeoService] Error saving document locally: {ex.Message}");
                return OperationResult.Failed($"Error saving document: {ex.Message}");
            }
        }
    }
}
