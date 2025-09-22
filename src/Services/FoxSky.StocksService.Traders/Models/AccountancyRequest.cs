using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.Traders.Models
{
    public class AccountancyRequest
    {
        public string Ticker { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public TradingAction TradingAction { get; set; }
        public DateTime DecisionTime { get; set; } = DateTime.UtcNow;
    }
}
