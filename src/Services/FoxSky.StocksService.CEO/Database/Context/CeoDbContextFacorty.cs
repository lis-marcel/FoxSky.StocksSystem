using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FoxSky.StocksService.Ceo.Database.Context
{
    public class CeoDbContextFacorty : IDesignTimeDbContextFactory<CeoDbContext>
    {
        public CeoDbContext CreateDbContext(string[] args) 
        {
            var optionsBuilder = new DbContextOptionsBuilder<CeoDbContext>();
            var connectionString = Environment.GetEnvironmentVariable("DB_HOST");

            object value = optionsBuilder.UseSqlite(connectionString);

            return new CeoDbContext(optionsBuilder.Options);
        }
    }
}
