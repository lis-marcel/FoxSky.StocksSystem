using System;

namespace FoxSky.StocksSystem.SharedServices.Models
{
    public class DmsReportModel
    {
        public Guid ReportId { get; set; }
        public byte[] Document { get; set; }
        public int IssuerId { get; set; }
        public int CommissionerId { get; set; }
        public string CommissionerEmail { get; set; }
        public DateTime IssueDate { get; set; }
    }
}