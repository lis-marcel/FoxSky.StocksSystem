using FoxSky.StocksSystem.Accountancy.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoxSky.StocksSystem.Accountancy.Database.Context
{
    public class AccountancyDbContext : DbContext
    {
        public DbSet<Trade> Trades { get; set; }
        public DbSet<Report> Reports { get; set; }

        public AccountancyDbContext(DbContextOptions<AccountancyDbContext> options) : base(options) { }
    }
}
