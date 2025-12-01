using FoxSky.StocksService.CEO.MessageBroker;
using FoxSky.StocksService.SharedServices.Models;
using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksService.CEO.Services
{
    public interface ICeoService
    {
        Task<OperationResult> RequestReportGeneration(ICeoMessageBroker messageBroker);
        Task<OperationResult> ReceiveReportData(string data);
        Task<OperationResult> DownloadReport();
        Task<OperationResult> DownloadReport(string reportId);
        Task<OperationResult> SaveDocumentLocally(DocumentDownloadResponse response);
    }
}
