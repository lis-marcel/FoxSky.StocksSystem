using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksService.CEO.Services
{
    public interface ICeoService
    {
        Task<OperationResult> ReceiveReportData(string data);
        Task<OperationResult> RequestReportGeneration();
        Task<OperationResult> DownloadReport(string reportId);
    }
}
