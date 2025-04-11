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
    public static void AddApiAuthentication(this IServiceCollection services, IConfigurationSection jwtSection)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                var secretKey = jwtSection.GetValue<string>("SecretKey");
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["cookies"];
                        return Task.CompletedTask;
                    }
                };
            });
        services.AddAuthorization();
    }
}