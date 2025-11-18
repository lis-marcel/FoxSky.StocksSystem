using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.Traders.Models;

namespace FoxSky.StocksSystem.Traders.Services
{
    public interface ITradersService
    {
        Task<OperationResult> ProcessStockDataAsync(StockData stockData);
        Task<OperationResult> ProcessStockDataAsync(string json);
    }
}
