using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.Accountancy.Database.Entities
{
    public class Report
    {
        public Guid ReportId { get; set; }
        public string DmsDocumentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
