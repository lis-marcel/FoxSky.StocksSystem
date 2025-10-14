using FoxSky.StocksService.SharedServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksService.CEO.Services
{
    public interface ICEOService
    {
        Task<OperationResult> RequestReport(DateTime beginningDate, DateTime endDate);
    }
}
