using FoxSky.StocksSystem.Accountancy.Database.Entities;

namespace FoxSky.StocksSystem.Accountancy.Models
{
    public class ReportModel
    {
        public int ReportId { get; set; }
        public string IssuerName { get; set; } = "CEO";
        public DateTime IssueDate { get; set; }
        public List<Trade>? Trades { get; set; }
    }
}
