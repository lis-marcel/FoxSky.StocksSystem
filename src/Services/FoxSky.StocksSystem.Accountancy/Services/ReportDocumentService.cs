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

                    page.Header().Element(ComposeHeader);

                    page.Content().Element(ComposeContent);

                    page.Footer().Element(ComposeFooter);
                });
        }

        private void ComposeHeader(IContainer container)
        {
            container
                .PaddingVertical(10)
                .Row(row =>
                {
                    row.RelativeItem().Column(column =>
                    {
                        column.Item().Text($"Report ID: {reportModel.ReportId}").FontSize(14);
                        column.Item().Text($"Issue Date: {reportModel.IssueDate:yyyy-MM-dd}").FontSize(14);
                        column.Spacing(5);
                        column.Item().Text($"Report Period: {reportModel.BeginningDate:yyyy-MM-dd} to {reportModel.EndDate:yyyy-MM-dd}").FontSize(16);
                        column.Spacing(5);
                        column.Item().Text($"Commissioned by: {reportModel.Commissioner!.CommissionerName}").FontSize(14);
                        column.Item().Text($"Issued by: {reportModel.Issuer!.IssuerName}").FontSize(14);
                    });
                });
        }

        private void ComposeContent(IContainer container)
        {
            container.PaddingVertical(40).Column(column =>
            {
                column.Spacing(5);

                column.Item().Element(ComposeTable);

                // Calculate buys and sells separately
                decimal totalBuys = reportModel.Trades!
                    .Where(t => t.TradingAction == TradingAction.Buy)
                    .Sum(t => t.Price * t.Quantity);

                decimal totalSells = reportModel.Trades!
                    .Where(t => t.TradingAction == TradingAction.Sell)
                    .Sum(t => t.Price * t.Quantity);

                decimal netCashFlow = totalSells - totalBuys; // Positive = profit, Negative = loss

                column.Item().PaddingTop(10).AlignRight()
                    .Text($"Total Buys: {totalBuys:C}").FontSize(12);
                column.Item().AlignRight()
                    .Text($"Total Sells: {totalSells:C}").FontSize(12);
                column.Item().AlignRight()
                    .Text($"Net Cash Flow: {netCashFlow:C}")
                    .FontSize(14).Bold()
                    .FontColor(netCashFlow >= 0 ? Colors.Green.Darken2 : Colors.Red.Darken2);
            });
        }

        private void ComposeTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.ConstantColumn(25);
                    columns.RelativeColumn(3);
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                    columns.RelativeColumn();
                });

                table.Header(header =>
                {
                    header.Cell().Element(CellStyle).Text("#").FontSize(12).Bold();
                    header.Cell().Element(CellStyle).Text("Ticker").FontSize(12).Bold();
                    header.Cell().Element(CellStyle).Text("Price").FontSize(12).Bold();
                    header.Cell().Element(CellStyle).Text("Quantity").FontSize(12).Bold();
                    header.Cell().Element(CellStyle).Text("Action").FontSize(12).Bold();
                    header.Cell().Element(CellStyle).Text("Decision Time").FontSize(12).Bold();

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                    }
                });

                foreach (var trade in reportModel.Trades!)
                {
                    table.Cell().Element(CellStyle).Text($"{trade.Id}").FontSize(12);
                    table.Cell().Element(CellStyle).Text($"{trade.Ticker}").FontSize(12);
                    table.Cell().Element(CellStyle).Text($"{trade.Price:C}").FontSize(12);
                    table.Cell().Element(CellStyle).Text($"{trade.Quantity}").FontSize(12);
                    table.Cell().Element(CellStyle).Text($"{trade.TradingAction}").FontSize(12);
                    table.Cell().Element(CellStyle).Text($"{trade.DecisionTime:yyyy-MM-dd HH:mm:ss}").FontSize(12);

                    static IContainer CellStyle(IContainer container)
                    {
                        return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                    }
                }
            });
        }

        private void ComposeFooter(IContainer container)
        {
            container
                .PaddingVertical(10)
                .AlignCenter()
                .Text("Generated by FoxSky Stocks System Accountancy Service")
                .FontSize(10);
        }
    }
}