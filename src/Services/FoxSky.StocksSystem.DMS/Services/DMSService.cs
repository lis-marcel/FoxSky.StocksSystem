using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.SharedServices.Models;

namespace FoxSky.StocksSystem.DMS.Services
{
    public class DMSService : IDMSService
    {
        public Task<OperationResult> ProcessSaveDocumentRequest(byte[] data)
        {
            //var deserializedData = Newtonsoft.Json.Converters.Convert.DeserializeObject<>(

            throw new NotImplementedException();
        }
    }
}
