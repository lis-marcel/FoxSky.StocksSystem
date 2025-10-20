using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.Accountancy.Models
{
    public class CommissionerDataModel
    {
        public int CommissionerId { get; set; }
        public string CommissionerName { get; set; } = "Max Verstappen";
        public string CommissionerEmail { get; set; } = "max.verstappen@ceo.mail.com";
        public string CommissionerPhone { get; set; } = "+1-202-555-0143";
    }
}
