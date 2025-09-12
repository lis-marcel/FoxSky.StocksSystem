using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FoxSky.StocksService.Traders.BOs
{
    public class StockData
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; } = string.Empty;

        [JsonPropertyName("open")]
        public decimal Open { get; set; }

        [JsonPropertyName("close")]
        public decimal Close { get; set; }

        [JsonPropertyName("high")]
        public decimal High { get; set; }

        [JsonPropertyName("low")]
        public decimal Low { get; set; }

        [JsonPropertyName("volume")]
        public long Volume { get; set; }

        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

        public decimal PriceChange => Close - Open;
        public decimal PriceChangePercent => Open != 0 ? (PriceChange / Open) * 100 : 0;
        public bool IsUpwardTrend => Close > Open;
        public decimal TypicalPrice => (High + Low + Close) / 3;
    }
}
