using System.Runtime.InteropServices;
using System.Text.Json.Serialization;

namespace FoxSky.StocksSystem.Traders.Models
{
    public class StockData
    {
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; } = string.Empty;

        [JsonPropertyName("queryCount")]
        public int QueryCount { get; set; }

        [JsonPropertyName("resultsCount")]
        public int ResultsCount { get; set; }

        [JsonPropertyName("adjusted")]
        public bool Adjusted { get; set; }

        [JsonPropertyName("results")]
        public Result[] Results { get; set; } = [];

        [JsonPropertyName("status")]
        public string Status { get; set; } = string.Empty;

        [JsonPropertyName("request_id")]
        public string Request_id { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }

    public class Result
    {
        [JsonPropertyName("T")]
        public string Ticker { get; set; } = string.Empty;
        [JsonPropertyName("v")]
        public decimal Volume { get; set; }
        [JsonPropertyName("vw")]
        public decimal VolumeWeight { get; set; }
        [JsonPropertyName("o")]
        public decimal Open { get; set; }
        [JsonPropertyName("c")]
        public decimal Close { get; set; }
        [JsonPropertyName("h")]
        public decimal High { get; set; }
        [JsonPropertyName("l")]
        public decimal Low { get; set; }
        [JsonPropertyName("t")]
        public long Timestamp { get; set; }
        [JsonPropertyName("n")]
        public int Number { get; set; }

        public decimal PriceChange => Close - Open;
        public decimal PriceChangePercent => Open != 0 ? PriceChange / Open * 100 : 0;
        public bool IsUpwardTrend => Close > Open;
        public decimal TypicalPrice => (High + Low + Close) / 3;
    }

}
