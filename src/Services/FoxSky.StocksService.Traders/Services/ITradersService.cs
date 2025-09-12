using FoxSky.StocksService.SharedServices;
using FoxSky.StocksService.Traders.BOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksService.Traders.Services
{
    public interface ITradersService
    {
        Task<OperationResult> ProcessStockDataAsync(StockData stockData);
        Task<OperationResult> ProcessStockDataAsync(string json);
    }
}
