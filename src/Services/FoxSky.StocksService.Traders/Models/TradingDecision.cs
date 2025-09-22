namespace FoxSky.StocksSystem.Traders.Models
{
    public class TradingDecision
    {
        public string Ticker { get; set; } = string.Empty;
        public TradingAction Action { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal Confidence { get; set; }
        public DateTime DecisionTime { get; set; } = DateTime.UtcNow;
    }

    public enum TradingAction
    {
        Buy,
        Sell,
        Hold
    }
}
