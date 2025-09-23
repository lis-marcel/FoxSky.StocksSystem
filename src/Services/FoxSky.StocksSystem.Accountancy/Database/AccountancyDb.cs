using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.Accountancy.Database
{
    public class AccountancyDb : DbContext
    {
        DbSet<Models.AccountancyRequest> AccountancyRequests { get; set; }
    }
}
