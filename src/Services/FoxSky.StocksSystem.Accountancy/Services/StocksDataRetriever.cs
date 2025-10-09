using FoxSky.StocksSystem.Accountancy.Database.Context;
using FoxSky.StocksSystem.Accountancy.Database.Entities;
using FoxSky.StocksSystem.Accountancy.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class StocksDataRetriever
    {
        public static async Task<List<Trade>> RetreiveTradesData(AccountancyDbContext dbContext, DateTime beginningDate, DateTime endDate)
        {
            var trades = await dbContext.Trades
                .Where(e => e.DecisionTime > beginningDate && e.DecisionTime < endDate)
                .ToListAsync();

            return trades;
        }
    }
}
