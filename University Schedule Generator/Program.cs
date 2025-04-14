using Bogus;
using LAB1_WEB_API;
using LAB1_WEB_API.Endpoints;
using LAB1_WEB_API.Infrastructure.Generators.Data;
using LAB1_WEB_API.Interfaces.DataSaver;
using LAB1_WEB_API.Services;
using LAB1_WEB_API.Services.DataSavers;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

// *** 1. Добавьте сервисы CORS ***
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins"; // Имя для политики
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            // Укажите ТОЧНЫЙ URL вашего ШЛЮЗА
            policy.WithOrigins("https://localhost:7249")
                .AllowAnyHeader() 
                .AllowAnyMethod(); 

        });
});
var services = builder.Services;
builder.Logging.AddConsole();
// Swagger/OpenAPI
services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "GeneratorService API", Version = "v1" });
});

//добавляем базы
services.AddPostgres(builder.Configuration);
services.AddNeo4j(builder.Configuration);
services.AddRedis(builder.Configuration);
services.AddMongoDb(builder.Configuration);
services.AddElastic(builder.Configuration);
//создаём сервис для генерации мусора
services.AddScoped<GeneratorService>();
services.AddScoped<IDataSaver<GeneratedData>, PostgresDataSaver>();
services.AddScoped<IDataSaver<GeneratedData>, Neo4jDataSaver>();
services.AddScoped<IDataSaver<GeneratedData>, MongoDataSaver>();
services.AddScoped<IDataSaver<GeneratedData>, RedisDataSaver>();
services.AddScoped<IDataSaver<GeneratedData>, ElasticDataSaver>();
//сервис сохранения мусора
services.AddScoped<DataSaverService>();


var app = builder.Build();
app.UseHttpsRedirection();
app.UseCors(MyAllowSpecificOrigins);
// Включаем middleware для Swagger
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "University Schedule API V1");
    options.RoutePrefix = ""; // Доступ по /
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseCookiePolicy(new CookiePolicyOptions()
{
    MinimumSameSitePolicy = SameSiteMode.Strict,
    Secure = CookieSecurePolicy.Always,
    HttpOnly = HttpOnlyPolicy.Always
});


//эндпоинты генерации данных
app.AddGeneratorEndPoints();

//регистрируем все базы данных
var group = app.MapGroup("/api/v1").RequireAuthorization();
group.AddDataBaseEndPoints();
app.Run();