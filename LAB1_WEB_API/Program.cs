using LAB1_WEB_API;
using LAB1_WEB_API.Endpoints;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication("Bearer").AddJwtBearer(); // схема аутентификации - с помощью jwt-токенов.
builder.Services.AddAuthorization();
// Добавляем Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); // <-- Добавьте эту строку

//добавляем базы
builder.Services.AddPostgres(builder.Configuration);
builder.Services.AddNeo4j(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddElastic(builder.Configuration);

var app = builder.Build();

// Включаем middleware для Swagger
app.UseSwagger(); // <-- Добавьте эту строку
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

var group = app.MapGroup("/api/v1");
group.
    MapPostgresEndpoints().
    MapElasticSearchEndpoints().
    MapNeo4jEndpoints().
    MapMongoEndpoints().
    MapRedisEndpoints().
    MapOpenApi();

app.Run();