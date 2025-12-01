namespace FoxSky.StocksService.SharedServices.Models
{
    public class DocumentDownloadRequest
    {
        public string DmsDocumentId { get; set; } = string.Empty;
        public Guid ReportId { get; set; }
    }
}
