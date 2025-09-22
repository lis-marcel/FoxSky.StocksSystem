using FoxSky.StocksSystem.Traders.Models;

namespace FoxSky.StocksSystem.Traders.Services
{
    public interface ITradingStrategy
    {
        Task<TradingDecision> AnalyzeStockAsync(StockData stockData);
        Task<TradingSignal> GenerateSignalAsync(StockData stockData);
    }
}
