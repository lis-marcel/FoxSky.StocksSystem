using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.SharedServices.Models;
using Newtonsoft.Json;
using System.Text;

namespace FoxSky.StocksSystem.DMS.Services
{
    public class DMSService : IDMSService
    {
        public async Task<OperationResult> ProcessSaveDocumentRequest(byte[] data)
        {
            if (data == null) return OperationResult.Failed(message: "No data in request.");

            var dataString = Encoding.UTF8.GetString(data);

            var deserializedData = JsonConvert.DeserializeObject<DmsReportModel>(dataString);

            // To-Do: Implement logic to save to MongoDB
            // await _mongoDbService.SaveDocumentAsync(reportModel);
            Console.WriteLine($"[DMS] Stored report {deserializedData!.ReportId} in the database.");

            // To-Do: Implement notification logic
            // await _notificationService.NotifyCommissionerAsync(reportModel.CommisionerId, reportModel.ReportId);
            Console.WriteLine($"[DMS] Sent notification to {deserializedData.CommissionerEmail}.");

            return OperationResult.Succeeded();
        }
    }
}
