using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksService.Ceo.Models
{
    internal class ReportResponseModel
    {
        public Guid ReportId { get; set; }
        public string DmsDocumentId { get; set; } = string.Empty;
    }
}
