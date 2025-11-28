using FoxSky.StocksService.Ceo.Database.Entities;
using Microsoft.EntityFrameworkCore;

namespace FoxSky.StocksService.Ceo.Database.Context
{
    public class CeoDbContext : DbContext
    {
        public DbSet<Report> Reports { get; set; }

        public CeoDbContext(DbContextOptions<CeoDbContext> options) : base(options) { }
    }
}
