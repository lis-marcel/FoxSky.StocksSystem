using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksSystem.DMS.Services
{
    internal interface IDMSService
    {
        Task<OperationResult> ProcessSaveDocumentRequest(byte[] data);
    }
}
