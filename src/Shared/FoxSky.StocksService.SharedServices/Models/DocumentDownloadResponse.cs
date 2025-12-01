namespace FoxSky.StocksService.SharedServices.Models
{
    public class DocumentDownloadResponse
    {
        public Guid ReportId { get; set; }
        public byte[] DocumentData { get; set; } = Array.Empty<byte>();
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
