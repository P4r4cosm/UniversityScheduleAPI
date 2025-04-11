using Bogus;
using LAB1_WEB_API;
using LAB1_WEB_API.Endpoints;
using LAB1_WEB_API.Repositories;
using LAB1_WEB_API.Services;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
// Swagger/OpenAPI
services.AddEndpointsApiExplorer();
services.AddSwaggerGen();


services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
//добавляем базы
services.AddPostgres(builder.Configuration);
services.AddNeo4j(builder.Configuration);
services.AddRedis(builder.Configuration);
services.AddMongoDb(builder.Configuration);
services.AddElastic(builder.Configuration);
//Сервисы для JWT
services.AddScoped<UserRepository>();
services.AddScoped<IJwtProvider, JwtProvider>();
services.AddScoped<UserServices>();
services.AddScoped<IPasswordHasher, PasswordHasher>();

//создаём сервис для генерации мусора
services.AddScoped<GeneratorService>();

services.AddApiAuthentication(builder.Configuration.GetSection("JwtOptions")); // схема аутентификации - с помощью jwt-токенов.
services.AddAuthorization();
var app = builder.Build();

// Включаем middleware для Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "University Schedule API V1");
    options.RoutePrefix = ""; // Доступ по /
});

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always
});

//регистрируем эндпоинты login register
app.MapUserEndpoints();

//эндпоинты генерации данных
app.AddGeneratorEndPoints();

//регистрируем все базы данных
var group = app.MapGroup("/api/v1").RequireAuthorization();
group.AddDataBaseEndPoints();
app.Run();