using System;

namespace FoxSky.StocksSystem.Accountancy.Models
{
    public class DmsReportModel
    {
        public Guid ReportId { get; set; }
        public byte[] Document { get; set; }
        public Guid IssuerId { get; set; }
        public Guid CommissionerId { get; set; }
        public string CommissionerEmail { get; set; }
        public DateTime IssueDate { get; set; }
    }
}