using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksSystem.DMS.Services
{
    public interface IDMSService
    {
        Task<OperationResult> ProcessSaveDocumentRequest(byte[] data);
    }
}
