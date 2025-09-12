using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksService.Traders.BOs
{
    public class TradingStrategies
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
