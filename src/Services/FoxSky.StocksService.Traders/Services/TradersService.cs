using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.Traders.BOs;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;

namespace FoxSky.StocksService.Traders.Services
{
    public class TradersService : ITradersService
    {
        private readonly ITradingStrategy _tradingStrategy;

        public TradersService(ITradingStrategy? tradingStrategy = null)
        {
            _tradingStrategy = tradingStrategy ?? new TradingStrategy();
        }

        public async Task<OperationResult> ProcessStockDataAsync(string message)
        {
            try
            {
                var stockData = System.Text.Json.JsonSerializer.Deserialize<StockData>(message);

                if (stockData == null)
                {
                    return OperationResult.Failed("Invalid stock data.");
                }

                return await ProcessStockDataAsync(stockData);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Error processing stock data: {ex.Message}");
            }
        }

        public async Task<OperationResult> ProcessStockDataAsync(StockData stockData)
        {
            try
            {
                Console.WriteLine($"Processing stock data for {stockData.Ticker} at {new DateTime(stockData.Results[0].Timestamp)}");
                Console.WriteLine(
                    $"Open: {stockData.Results[0].Open}," +
                    $" Close: {stockData.Results[0].Close}," +
                    $" High: {stockData.Results[0].High}," +
                    $" Low: {stockData.Results[0].Low}," +
                    $" Volume: {stockData.Results[0].Volume}");

                var signal = await _tradingStrategy.GenerateSignalAsync(stockData);
                Console.WriteLine($"Generated Signal: {signal.Type} with confidence {signal.Strength:F1}% - {signal.Description}");

                var decision = await _tradingStrategy.AnalyzeStockAsync(stockData);
                Console.WriteLine($"[TradersService] Decision: {decision.Action} {decision.Quantity} shares at ${decision.Price}");
                Console.WriteLine($"[TradersService] Reason: {decision.Reason} (Confidence: {decision.Confidence:F1}%)");

                var result = new
                {
                    StockData = stockData,
                    TradingSignal = signal,
                    TradingDecision = decision
                };

                return OperationResult.Succeeded("Stock data processed successfully.", result);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Error processing stock data: {ex.Message}");
            }
        }
    }
}
