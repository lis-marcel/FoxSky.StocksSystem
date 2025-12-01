using FoxSky.StocksSystem.SharedServices;

namespace FoxSky.StocksSystem.DMS.Services
{
    public interface IDmsService
    {
        Task<OperationResult> ProcessSaveDocumentRequestAsync(byte[] data);
    }
}
