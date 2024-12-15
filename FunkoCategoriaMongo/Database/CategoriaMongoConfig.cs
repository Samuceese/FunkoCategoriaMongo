using MongoDB.Bson;
using MongoDB.Driver;

namespace FunkoCategoriaMongo.Database;

public class CategoriaMongoConfig
{
    private readonly ILogger _logger;

    public CategoriaMongoConfig(ILogger<CategoriaMongoConfig> logger)
    {
        _logger = logger;
    }
    
    public CategoriaMongoConfig(){}
    
    public string ConnectionString { get; set; } = String.Empty;
    public string DatabaseName { get; set; } = String.Empty;
    public string CategoriaCollectionName { get; set; } = String.Empty;

    public void TryConnection()
    {
        _logger.LogInformation("Trying to connect to MongoDB");
        var settings = MongoClientSettings.FromConnectionString(ConnectionString);
        // Set the ServerApi field of the settings object to set the version of the Stable API on the client
        settings.ServerApi = new ServerApi(ServerApiVersion.V1);
        // Create a new client and connect to the server
        var client = new MongoClient(settings);
        // Send a ping to confirm a successful connection
        try
        {
            client.GetDatabase("DatabaseName").RunCommand<BsonDocument>(new BsonDocument("ping", 1));
            _logger.LogInformation("🟢 You successfully connected to MongoDB!");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "🔴 Error connecting to MongoDB");
            Environment.Exit(1);
        }
    }
}