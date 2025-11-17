using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.Accountancy.Models
{
    public class IssuerDataModel
    {
        public int IssuerId { get; set; } = 1;
        public string IssuerName { get; set; } = "Nikola Tesla";
        public string IssuerEmail { get; set; } = "nikola.tesla@accountancy.mail.com";
        public string IssuerPhone { get; set; } = "+1234567890";
    }
}
