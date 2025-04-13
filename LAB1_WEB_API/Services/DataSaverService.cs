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
    private readonly ILogger<DataSaverService> _logger;
    private readonly IEnumerable<IDataSaver<GeneratedData>> _dataSavers;

    public DataSaverService(IEnumerable<IDataSaver<GeneratedData>> savers, ILogger<DataSaverService> logger, GeneratorService generator)
    {
        _generator = generator;
        _logger = logger;
        _dataSavers = savers;
    }

    public async Task<string> GenerateAndSaveDataAsync()
    {
        var sbReport = new StringBuilder("Generation and Saving Report:\n");
        try
        {
           
            var generatedData = _generator.GenerateForPostgres();
            
            //сначала сохраняем в postgres, чтобы не было проблем с id при генерации текстов
            //после во все остальные базы 
            var pgSaver = _dataSavers.OfType<PostgresDataSaver>().FirstOrDefault();
            if (pgSaver == null) throw new InvalidOperationException("PostgresDataSaver not found.");
            
            var pgResult=await pgSaver.SaveAsync(generatedData);
            generatedData = _generator.GenerateDataForElastic(generatedData);
            
            var otherSavers = _dataSavers.Where(s => s != pgSaver); // Исключаем уже выполненный pgSaver
            foreach (var saver in otherSavers)
            {
                await saver.SaveAsync(generatedData);
            }
            
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