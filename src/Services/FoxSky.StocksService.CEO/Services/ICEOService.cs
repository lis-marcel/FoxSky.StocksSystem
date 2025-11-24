using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksService.CEO.Services
{
    public interface ICEOService
    {
        void ReceiveReportData(string data);
    }
}
