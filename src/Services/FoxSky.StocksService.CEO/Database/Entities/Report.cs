namespace FoxSky.StocksService.Ceo.Database.Entities
{
    public class Report
    {
        public Guid ReportId { get; set; }
        public string DmsDocumentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
