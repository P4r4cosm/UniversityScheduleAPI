using Elastic.Clients.Elasticsearch;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Neo4j.Driver;
using StackExchange.Redis;

namespace LAB1_WEB_API;

public static class ServiceDataBaseExtensions
{
    public static void AddPostgres(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<ApplicationContext>(options =>
            options.UseNpgsql(config.GetConnectionString("DefaultConnection")));
    }

    public static void AddRedis(this IServiceCollection services, IConfiguration config)
    {
        var redis = ConnectionMultiplexer.Connect(config["RedisOptions:Configuration"]);
        services.AddSingleton(redis.GetDatabase());
    }

    public static void AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        var mongoSettings = config.GetSection("MongoDbSettings");
        var mongoClient = new MongoClient(mongoSettings["ConnectionString"]);
        var mongoDatabase = mongoClient.GetDatabase(mongoSettings["Database"]);
        services.AddSingleton<IMongoDatabase>(mongoDatabase);
    }

    public static void AddNeo4j(this IServiceCollection services, IConfiguration config)
    {
        var neo4jOptions = config.GetSection("Neo4jOptions");
        var neo4jUri = neo4jOptions["Uri"];
        var neo4jUsername = neo4jOptions["Username"];
        var neo4jPassword = neo4jOptions["Password"];
        var neo4jDriver = GraphDatabase.Driver(neo4jUri, AuthTokens.Basic(neo4jUsername, neo4jPassword));
        services.AddSingleton<IDriver>(neo4jDriver);
    }

    public static void AddElastic(this IServiceCollection services, IConfiguration config)
    {
        string esUri = config["ElasticsearchOptions:Uri"];
        var esSettings = new ElasticsearchClientSettings(new Uri(esUri))
            .DefaultIndex("materials"); // optional
        services.AddSingleton(new ElasticsearchClient(esSettings));
    }
}