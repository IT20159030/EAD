

using MongoDB.Driver;

namespace Backend.Utils;
public class MongoDBService
{
  private readonly IConfiguration _configuration;
  private readonly IMongoDatabase _database;

  public MongoDBService(IConfiguration configuration)
  {
    _configuration = configuration;

    var connectionString = _configuration.GetConnectionString("MongoDB");

    if (string.IsNullOrEmpty(connectionString))
    {
      throw new InvalidOperationException("Cannot read MongoDB connection settings");
    }

    var mongoUrl = MongoUrl.Create(connectionString);
    var mongoClient = new MongoClient(mongoUrl);

    _database = mongoClient.GetDatabase(mongoUrl.DatabaseName);
  }

  public IMongoDatabase Database => _database;
}