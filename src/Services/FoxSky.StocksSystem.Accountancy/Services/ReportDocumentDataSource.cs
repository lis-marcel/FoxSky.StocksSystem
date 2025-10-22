using FoxSky.StocksSystem.Accountancy.Database.Context;
using FoxSky.StocksSystem.Accountancy.Database.Entities;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class ReportDocumentDataSource
    {
        public static async Task<List<Trade>> RetreiveTradesData(AccountancyDbContext dbContext, DateTime beginningDate)
        {
            var dateOnly = DateOnly.FromDateTime(beginningDate);

            var tradesList = await dbContext.Trades
               .Where(e => 
                    e.DecisionTime.Year == dateOnly.Year 
                    && e.DecisionTime.Month == dateOnly.Month 
                    && e.DecisionTime.Day == dateOnly.Day)
               .AsNoTracking()
               .ToListAsync();

            return tradesList;
        }

        public static async Task<List<Trade>> RetreiveTradesData(AccountancyDbContext dbContext, DateTime beginningDate, DateTime endDate)
        {
             var tradesList = await dbContext.Trades
                .Where(e => e.DecisionTime >= beginningDate && e.DecisionTime <= endDate)
                .AsNoTracking()
                .ToListAsync();

            return tradesList;
        }
    }
}
