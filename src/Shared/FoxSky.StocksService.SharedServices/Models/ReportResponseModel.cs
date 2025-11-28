using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksService.SharedServices.Models
{
    public class ReportResponseModel
    {
        public Guid ReportId { get; set; }
        public string DmsDocumentId { get; set; } = string.Empty;
    }
}
