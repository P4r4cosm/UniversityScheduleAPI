using System.Text;
using LAB1_WEB_API.Endpoints;
using LAB1_WEB_API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace LAB1_WEB_API;

public static class ApiExtensions
{
    public static void AddDataBaseEndPoints(this IEndpointRouteBuilder app)
    {
        app
            .MapRedisEndpoints()
            .MapPostgresEndpoints()
            .MapElasticSearchEndpoints()
            .MapMongoEndpoints()
            .MapNeo4jEndpoints();
    }

    public static void AddGeneratorEndPoints(this IEndpointRouteBuilder app)
    {
        app.MapGeneratorEndpoints();
    }
}