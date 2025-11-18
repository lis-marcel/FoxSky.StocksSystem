using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksService.CEO.Services
{
    public class CEOService : ICEOService
    {
        private readonly IServiceProvider _serviceProvider;

        public CEOService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public Task<OperationResult> RequestReport(DateTime beginningDate, DateTime endDate)
        {
            return Task.FromResult(OperationResult.Succeeded("Report request received.", new { BeginningDate = beginningDate, EndDate = endDate }));
        }
    }
}
