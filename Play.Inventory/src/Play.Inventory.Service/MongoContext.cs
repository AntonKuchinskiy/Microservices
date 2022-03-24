using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Play.Common.Settings;

namespace Play.Inventory.Service.Dtos
{
    public class MongoContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public MongoContext(IOptions<ServiceSettings> serviceSettings, IOptions<MongoDbSettings> mongoDbSettings)
        {
            var _serviceSettingsValue = serviceSettings.Value;
            var _mongoDbSettingsValue = mongoDbSettings.Value;

            _client = new MongoClient(_mongoDbSettingsValue.ConnectionString);
            _database = _client.GetDatabase(_serviceSettingsValue.ServiceName);
        }

        public IMongoClient Client => _client;

        public IMongoDatabase Database => _database;
    }
}