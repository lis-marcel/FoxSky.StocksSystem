using FoxSky.StocksService.Traders.BOs;

namespace FoxSky.StocksService.Traders.Services
{
    public interface ITradingStrategy
    {
        Task<TradingDecision> AnalyzeStockAsync(StockData stockData);
        Task<TradingSignal> GenerateSignalAsync(StockData stockData);
    }
}
