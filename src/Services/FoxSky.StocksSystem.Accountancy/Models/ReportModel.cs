using FoxSky.StocksSystem.Accountancy.Database.Entities;

namespace FoxSky.StocksSystem.Accountancy.Models
{
    public class ReportModel
    {
        public int ReportId { get; set; }
        public CommissionerDataModel? Commissioner { get; set; }
        public IssuerDataModel? Issuer { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime BeginDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<Trade>? Trades { get; set; }
    }
}
