using FoxSky.StocksSystem.SharedServices;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoxSky.StocksSystem.DMS.Database.Context
{
    public class DMSDbContextFactory
    {
        public static async Task<OperationResult> CheckDbEnviromentAsync(MongoClient client, string dbName, string collection)
        {
            try
            {
                var database = client.GetDatabase(dbName);
                var collectionNames = database.ListCollectionNames().ToList();

                if (!collectionNames.Contains(collection))
                {
                    Console.WriteLine($"Collection '{collection}' does not exist in database '{dbName}'.");
                    await database.CreateCollectionAsync(collection);
                    Console.WriteLine($"Collection '{collection}' has been created.");
                }

                return OperationResult.Succeeded("Database environment is properly configured.");
            }
            catch (Exception ex)
            {
                return OperationResult.Failed($"Database environment check failed: {ex.Message}");
            }
        }
    }
}
