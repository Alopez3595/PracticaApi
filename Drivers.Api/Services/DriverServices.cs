using Drivers.Api.Configurations;
using Drivers.Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Drivers.Api.Services;

public class DriverServices
{
    private readonly IMongoCollection<Drive> _driversCollection;
    public DriverServices(
        IOptions<DatabaseSettings> databaseSettings)
    {
       //Inicializar mi cliente de MongoDB
       var mongoClient = new MongoClient(databaseSettings.Value.ConnectionString); 
       //Conectar a la base de dato de MongoDB
       var mongoDB = 
       mongoClient.GetDatabase(databaseSettings.Value.DatabaseName);
       _driversCollection = 
        mongoDB.GetCollection<Drive>
            (databaseSettings.Value.CollectionName);
    } 

    public async Task<List<Drive>> GetAsync() => 
        await _driversCollection.Find(_ => true).ToListAsync();

    public async Task<Drive> GetDriverById(string id)
    {
        return await _driversCollection.FindAsync(new BsonDocument{{"_id", new ObjectId(id)}}).Result.FirstAsync();
    }

    public async Task InsertDriver(Drive driver)
    {
        await _driversCollection.InsertOneAsync(driver);
    }

        public async Task UpdateDriver(Drive driver)
    {
        var filter = Builders<Drive>.Filter.Eq(s=>s.Id, driver.Id);

        await _driversCollection.ReplaceOneAsync(filter, driver);
    }

    public async Task DeleteDriver(string id)
    {
        var filter = Builders<Drive>.Filter.Eq(s=>s.Id, id);
        await _driversCollection.DeleteOneAsync(filter);
    }

    
}