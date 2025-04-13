using LAB1_WEB_API.Contracts.Users;
using LAB1_WEB_API.Services;

namespace LAB1_WEB_API.Endpoints;

public static class GeneratorEndpoints
{
    public static IEndpointRouteBuilder MapGeneratorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("generate", GenerateAndSaveData);
        return app;
    }

    public static async Task<IResult> GenerateAndSaveData(DataSaverService dataSaverService)
    {
        await dataSaverService.GenerateAndSaveDataAsync();
        return Results.Ok();
    }
}