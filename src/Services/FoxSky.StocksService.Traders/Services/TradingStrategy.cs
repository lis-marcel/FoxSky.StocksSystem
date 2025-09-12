using FoxSky.StocksService.Traders.BOs;

namespace FoxSky.StocksService.Traders.Services;

public class TradingStrategy : ITradingStrategy
{
    private readonly decimal _buyThreshold = 2.0m; // Buy if price increased by 2%
    private readonly decimal _sellThreshold = -1.5m; // Sell if price decreased by 1.5%
    private readonly long _minimumVolume = 1000; // Minimum volume threshold

    public async Task<TradingDecision> AnalyzeStockAsync(StockData stockData)
    {
        await Task.Delay(50); // Simulate analysis time

        var decision = new TradingDecision
        {
            Ticker = stockData.Ticker,
            Price = stockData.Close,
            DecisionTime = DateTime.UtcNow
        };

        // Simple strategy based on price change and volume
        if (stockData.Volume < _minimumVolume)
        {
            decision.Action = TradingAction.Hold;
            decision.Reason = "Insufficient volume";
            decision.Confidence = 30;
            decision.Quantity = 0;
        }
        else if (stockData.PriceChangePercent >= _buyThreshold)
        {
            decision.Action = TradingAction.Buy;
            decision.Reason = $"Strong upward momentum: {stockData.PriceChangePercent:F2}%";
            decision.Confidence = Math.Min(90, 50 + stockData.PriceChangePercent * 10);
            decision.Quantity = CalculateQuantity(stockData, TradingAction.Buy);
        }
        else if (stockData.PriceChangePercent <= _sellThreshold)
        {
            decision.Action = TradingAction.Sell;
            decision.Reason = $"Downward trend detected: {stockData.PriceChangePercent:F2}%";
            decision.Confidence = Math.Min(90, 50 + Math.Abs(stockData.PriceChangePercent) * 10);
            decision.Quantity = CalculateQuantity(stockData, TradingAction.Sell);
        }
        else
        {
            decision.Action = TradingAction.Hold;
            decision.Reason = "Price movement within acceptable range";
            decision.Confidence = 60;
            decision.Quantity = 0;
        }

        return decision;
    }

    public async Task<TradingSignal> GenerateSignalAsync(StockData stockData)
    {
        await Task.Delay(25); // Simulate signal generation time

        var signal = new TradingSignal
        {
            Symbol = stockData.Ticker
        };

        if (stockData.PriceChangePercent > 1.0m)
        {
            signal.Type = SignalType.Bullish;
            signal.Strength = Math.Min(100, stockData.PriceChangePercent * 20);
            signal.Description = "Positive price momentum detected";
        }
        else if (stockData.PriceChangePercent < -1.0m)
        {
            signal.Type = SignalType.Bearish;
            signal.Strength = Math.Min(100, Math.Abs(stockData.PriceChangePercent) * 20);
            signal.Description = "Negative price momentum detected";
        }
        else
        {
            signal.Type = SignalType.Neutral;
            signal.Strength = 40;
            signal.Description = "Sideways movement";
        }

        return signal;
    }

    private int CalculateQuantity(StockData stockData, TradingAction action)
    {
        // Simple quantity calculation based on confidence and price
        var baseQuantity = action == TradingAction.Buy ? 100 : 50;
        var volumeMultiplier = stockData.Volume > 10000 ? 1.5 : 1.0;
        return (int)(baseQuantity * volumeMultiplier);
    }
}