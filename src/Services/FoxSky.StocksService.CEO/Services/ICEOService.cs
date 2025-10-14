using FoxSky.StocksService.SharedServices;

namespace FoxSky.StocksService.CEO.Services
{
    public interface ICEOService
    {
        Task<OperationResult> RequestReport(DateTime beginningDate, DateTime endDate);
    }
}
