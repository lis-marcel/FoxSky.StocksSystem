using FoxSky.StocksSystem.Accountancy.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoxSky.StocksSystem.Accountancy.Database.Context
{
    public class AccountancyDb : DbContext
    {
        DbSet<Trade> Trades { get; set; }
    }
}
