using FoxSky.StocksSystem.DMS.Database.Context;
using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.SharedServices.Models;
using MongoDB.Driver.GridFS;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System.Text;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

namespace FoxSky.StocksSystem.DMS.Services
{
    public class DMSService : IDMSService
    {
        private readonly DMSDbContext<DmsReportModel> _dbContext;

        public DMSService(DMSDbContext<DmsReportModel> dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
        }

        public async Task<OperationResult> ProcessSaveDocumentRequest(byte[] data)
        {
            try
            {
                return await SaveDocumentAsync(data);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(message: $"Error processing document request: {ex.Message}");
            }
        }

        private async Task<OperationResult> SaveDocumentAsync(byte[] data)
        {
            try
            {
                var jsonString = Encoding.UTF8.GetString(data);
                var reportModel = JsonConvert.DeserializeObject<DmsReportModel>(jsonString);

                if (reportModel == null)
                {
                    return OperationResult.Failed("Deserialized report model is null.");
                }

                var gridFS = new GridFSBucket(_dbContext.Database);
                var documentId = await gridFS.UploadFromBytesAsync(Guid.NewGuid().ToString(), reportModel.Document);

                reportModel.DocumentId = documentId;

                await _dbContext.Collection.InsertOneAsync(reportModel);
                return OperationResult.Succeeded("Document saved successfully.");
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Error saving document: {ex.Message}");
            }
        }
    }
}
