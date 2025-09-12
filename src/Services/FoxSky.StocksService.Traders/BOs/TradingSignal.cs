namespace FoxSky.StocksService.Traders.BOs
{
    public class TradingSignal
    {
        public string Symbol { get; set; } = string.Empty;
        public SignalType Type { get; set; }
        public decimal Strength { get; set; } // 0-100
        public string Description { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;
    }

    public enum SignalType
    {
        Bullish,
        Bearish,
        Neutral
    }
}