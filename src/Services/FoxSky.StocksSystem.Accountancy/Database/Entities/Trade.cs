using FoxSky.StocksSystem.Accountancy.Models;
using System.ComponentModel.DataAnnotations;

namespace FoxSky.StocksSystem.Accountancy.Database.Entities
{
    public class Trade
    {
        [Key]
        public int Id { get; set; }
        public string Ticker { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public TradingAction TradingAction { get; set; }
        public DateTime DecisionTime { get; set; }
    }
}
