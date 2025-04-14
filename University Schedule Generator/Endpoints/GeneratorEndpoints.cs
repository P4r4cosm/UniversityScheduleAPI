using LAB1_WEB_API.Contracts.Generator;
using LAB1_WEB_API.Services;

namespace LAB1_WEB_API.Endpoints;

public static class GeneratorEndpoints
{
    public static IEndpointRouteBuilder MapGeneratorEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("generate", GenerateAndSaveData);
        return app;
    }

    public static async Task<IResult> GenerateAndSaveData(GenerateRequest request, DataSaverService dataSaverService)
    {
        await dataSaverService.GenerateAndSaveDataAsync(request.SpecialtiesCount,
            request.UniversityCount,
            request.InstitutionCount, 
            request.DepartmentCount, 
            request.GroupCount, 
            request.StudentCount,
            request.CourseCount);
        return Results.Ok();
    }
}