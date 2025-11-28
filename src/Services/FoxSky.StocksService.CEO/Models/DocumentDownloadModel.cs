namespace FoxSky.StocksService.Ceo.Models
{
    public class DocumentDownloadModel
    {
        public string DmsDocumentId { get; set; } = string.Empty;
        public Guid ReportId { get; set; }
    }
}
