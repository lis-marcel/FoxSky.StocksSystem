using FoxSky.StocksSystem.DMS.Database.Context;
using FoxSky.StocksSystem.SharedServices;
using FoxSky.StocksSystem.SharedServices.Models;
using MongoDB.Driver.GridFS;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using System.Text;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using System.Diagnostics;

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
                var processingResult = await SaveDocumentAsync(data);

                if (processingResult.Success)
                {
                    // TODO: send message to oderer service to notify about new document

                    return OperationResult.Succeeded(message: processingResult.Message);
                }

                return processingResult;
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(message: $"Error processing document request: {ex.Message}");
            }
        }

        public async Task<byte[]?> GetDocumentAsync(ObjectId documentId)
        {
            try
            {
                var gridFS = new GridFSBucket(_dbContext.Database);
                return await gridFS.DownloadAsBytesAsync(documentId);
            }
            catch (GridFSFileNotFoundException)
            {
                return null;
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
                return OperationResult.Succeeded($"Document saved successfully with ID: {documentId}", documentId);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Error saving document: {ex.Message}");
            }
        }
    }
}
