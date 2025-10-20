using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class ReportDocumentService : IDocument
    {
        public ReportModel reportModel { get; }

        public ReportDocumentService(ReportModel reportModel)
        {
            this.reportModel = reportModel;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
        public DocumentSettings GetSetting() => DocumentSettings.Default;

        public void Compose(IDocumentContainer container)
        {
            container
                .Page(page =>
                {
                    page.Margin(50);

                    page.Header().Height(100).Background(Colors.Grey.Lighten1);
                    page.Content().Background(Colors.Grey.Lighten3);
                    page.Footer().Height(50).Background(Colors.Grey.Lighten1);
                });
        }
    }
}