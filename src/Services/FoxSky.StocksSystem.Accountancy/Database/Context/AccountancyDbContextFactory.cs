using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksSystem.Accountancy.Database.Context
{
    public class AccountancyDbContextFactory : IDesignTimeDbContextFactory<AccountancyDbContext>
    {
        public AccountancyDbContext CreateDbContext(string[] args)
        {
            // Load environment variables
            ConfigVariablesReader.LoadEnv();

            var optionsBuilder = new DbContextOptionsBuilder<AccountancyDbContext>();
            var connectionString = Environment.GetEnvironmentVariable("DB_HOST");
            
            optionsBuilder.UseSqlite(connectionString);

            return new AccountancyDbContext(optionsBuilder.Options);
        }
    }
}