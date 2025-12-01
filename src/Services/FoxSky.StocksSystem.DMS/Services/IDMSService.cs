using FoxSky.StocksSystem.SharedServices;
using MongoDB.Bson;

namespace FoxSky.StocksSystem.DMS.Services
{
    public interface IDmsService
    {
        Task<OperationResult> ProcessSaveDocumentRequestAsync(byte[] data);
        Task<byte[]?> GetDocumentAsync(ObjectId documentId);
    }
}
