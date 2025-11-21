using MongoDB.Bson;
using MongoDB.Driver;

namespace FoxSky.StocksSystem.DMS.Database.Context
{
    public class DMSDbContext<T> where T : class
    {
        public readonly MongoClient Client;
        public IMongoDatabase Database;
        public IMongoCollection<T> Collection;

        public DMSDbContext(string serviceAddress, string dbName, string collection) 
        {
            try 
            {
                Client = new MongoClient($"mongodb://{serviceAddress}");

                if (!DMSDbContextFactory.CheckDbEnviromentAsync(Client, dbName, collection).GetAwaiter().GetResult().Success)
                {
                    throw new Exception("Database environment is not properly configured.");
                }

                Database = Client.GetDatabase(dbName);
                Collection = Database.GetCollection<T>(collection);
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to initialize DMSDbContextFactory.", ex);
            }
        }
    }
}
