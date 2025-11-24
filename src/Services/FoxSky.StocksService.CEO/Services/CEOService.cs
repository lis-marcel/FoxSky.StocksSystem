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

        public void ReceiveReportData(string data)
        {
            Console.WriteLine($"[CEOService] Received report ID: {data}");
        }
    }
}
