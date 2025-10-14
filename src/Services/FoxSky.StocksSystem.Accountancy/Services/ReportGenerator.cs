using FoxSky.StocksService.SharedServices;
using FoxSky.StocksSystem.Accountancy.Database.Entities;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.Accountancy.Services
{
    public class ReportGenerator
    {
        public static async Task<OperationResult> CreateReportFile(string range)
        {
            return await Task.FromResult(new OperationResult { Success = true, Message = "Report generated successfully." });
        }
    }
}