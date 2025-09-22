using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.Traders.Models
{
    public class AccountancyRequest(string ticker, int quantity, decimal price, TradingAction tradingAction, DateTime decistionTime)
    {
        public string Ticker { get; set; } = ticker;
        public int Quantity { get; set; } = quantity;
        public decimal Price { get; set; } = price;
        public TradingAction TradingAction { get; set; } = tradingAction;
        public DateTime DecisionTime { get; set; } = decistionTime;
    }
}
