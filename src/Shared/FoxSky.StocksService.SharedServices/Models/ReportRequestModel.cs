namespace FoxSky.StocksService.SharedServices.Models
{
    public class ReportRequestModel
    {
        public Guid ReportId { get; set; }
        public int CommissionerId { get; set; } 
        public DateTime BeginningDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
