using System.Text;
using LAB1_WEB_API.Infrastructure.Generators.Data;
using LAB1_WEB_API.Interfaces.DataSaver;
using LAB1_WEB_API.Interfaces.Generator;
using LAB1_WEB_API.Repositories;
using LAB1_WEB_API.Services.DataSavers;

namespace LAB1_WEB_API.Services;

public class DataSaverService
{
    private readonly GeneratorService _generator;
    // private readonly PostgresDataSaver _pgSaver;
    // private readonly ElasticDataSaver _elasticSaver;
    // private readonly RedisDataSaver _redisSaver;
    // private readonly MongoDataSaver _mongoSaver;
    // private readonly Neo4jDataSaver _neo4jSaver;
    private readonly ILogger<DataSaverService> _logger;
    private readonly IEnumerable<IDataSaver<GeneratedData>> _dataSavers;

    public DataSaverService(IEnumerable<IDataSaver<GeneratedData>> savers, ILogger<DataSaverService> logger, GeneratorService generator)
    {
        _generator = generator;
        // _pgSaver = pgSaver;
        // _elasticSaver = elasticSaver;
        // _redisSaver = redisSaver;
        // _mongoSaver = mongoSaver;
        // _neo4jSaver = neo4jSaver;
        _logger = logger;
        _dataSavers = savers;
    }

    public async Task<string> GenerateAndSaveDataAsync()
    {
        var sbReport = new StringBuilder("Generation and Saving Report:\n");
        try
        {
            // _logger.LogInformation("Starting data generation...");
             var generatedData = _generator.GenerateForPostgres();
            // _logger.LogInformation("Data generation complete.");
            // sbReport.AppendLine("- Data generated successfully.");
            //
            // _logger.LogInformation("Starting data saving sequence...");
            //
            // // 1. Save to PostgreSQL (Updates IDs in generatedData automatically)
            // var pgResult = await _pgSaver.SaveAsync(generatedData);
            // LogAndAppendResult(sbReport, "PostgreSQL", pgResult);
            // if (!pgResult.Success)
            //     throw pgResult.Error ?? new Exception("PostgreSQL save failed."); // Или другая логика обработки ошибок
            //
            // //Generate For Elastic
            // _logger.LogInformation("Starting data generation for elastic...");
            generatedData = _generator.GenerateDataForElastic(generatedData);
            foreach (var saver in _dataSavers)
            {
                await saver.SaveAsync(generatedData);
            }
            // _logger.LogInformation("Elastic Data generation complete.");
            // sbReport.AppendLine("-Elastic Data generated successfully.");
            //     
            // // 2. Save to Elasticsearch
            // var esResult = await _elasticSaver.SaveAsync(generatedData);
            // LogAndAppendResult(sbReport, "Elasticsearch", esResult);
            // if (!esResult.Success)
            //     throw esResult.Error ??
            //           new Exception("Elasticsearch save failed."); // Или другая логика обработки ошибок
            //
            // // 3. Save to Redis
            // var redisResult = await _redisSaver.SaveAsync(generatedData);
            // LogAndAppendResult(sbReport, "Redis", redisResult);
            // if (!redisResult.Success) throw redisResult.Error ?? new Exception("Redis save failed.");
            //
            // // 4. Save to MongoDB
            // var mongoResult = await _mongoSaver.SaveAsync(generatedData);
            // LogAndAppendResult(sbReport, "Mongo", mongoResult);
            // if (!mongoResult.Success) throw mongoResult.Error ?? new Exception("Mongo save failed.");
            //
            // // 5. Save to Neo4j
            // var neo4jResult = await _neo4jSaver.SaveAsync(generatedData);
            // LogAndAppendResult(sbReport, "Neo4j", neo4jResult);
            // if (!neo4jResult.Success) throw neo4jResult.Error ?? new Exception("Neo4j save failed.");
            //
            // _logger.LogInformation("Data saving finished.");
            // sbReport.AppendLine("\nFinished successfully."); // Если все шаги успешны
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during the process.");
            sbReport.AppendLine($"\n!!! OVERALL ERROR: {ex.Message} !!!");
            return sbReport.ToString();
        }

        return sbReport.ToString();
    }

    private void LogAndAppendResult(StringBuilder sb, string dbName, SaveResult result)
    {
        if (result.Success)
        {
            _logger.LogInformation($"{dbName} save successful. {result.Message}");
            sb.AppendLine($"- Data saved to {dbName}. {result.Message}");
        }
        else
        {
            _logger.LogError(result.Error, $"Error saving to {dbName}: {result.Message}");
            sb.AppendLine($"! Error saving to {dbName}: {result.Message}");
        }
    }
}