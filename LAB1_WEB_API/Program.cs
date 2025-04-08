using LAB1_WEB_API;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Elastic.Clients.Elasticsearch;

var builder = WebApplication.CreateBuilder(args);
// builder.Services.AddAuthentication("Bearer") // схема аутентификации - с помощью jwt-токенов
//     .AddJwtBearer();
// builder.Services.AddAuthorization();
builder.Services.AddOpenApi();

#region configurationDatabase

//Данные для подключения

//postgres
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

//redis
string redisConf = builder.Configuration["RedisOptions:Configuration"];
if (string.IsNullOrEmpty(redisConf))
{
    throw new InvalidOperationException("Redis configuration is missing or invalid.");
}

// Elasticsearch
string esUri = builder.Configuration["ElasticsearchOptions:Uri"];
if (string.IsNullOrEmpty(esUri))
{
    throw new InvalidOperationException("Elasticsearch configuration is missing or invalid.");
}

#endregion




#region databaseConnections
// Создание соединения с Redis

var redis = ConnectionMultiplexer.Connect(redisConf);
var db = redis.GetDatabase();

//соединение с postgres
builder.Services.AddDbContext<ApplicationContext>(options => options.UseNpgsql(connectionString));

// Elasticsearch client
var esSettings = new ElasticsearchClientSettings(new Uri(esUri))
    .DefaultIndex("materials"); // optional
builder.Services.AddSingleton(new ElasticsearchClient(esSettings));

#endregion

var app = builder.Build();
// app.UseAuthentication();
// app.UseAuthorization();
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();


app.MapGet("/generate", () => "OK")
    .WithName("Status");
app.MapGet("/users_from_pg", (ApplicationContext db) => db.Users.ToList());

app.MapGet("/redis_test", async () =>
{
    // Получаем хэш student:1
    var hashEntries = await db.HashGetAllAsync("student:1");

    // Преобразуем результат в словарь
    var result = hashEntries.ToDictionary(
        entry => entry.Name.ToString(),
        entry => entry.Value.ToString()
    );

    return result;
});
app.MapGet("/elastic_test", async (ElasticsearchClient esClient) =>
{
    var response = await esClient.SearchAsync<dynamic>(s => s
        .Index("materials")
        .Query(q => q
            .Match(m => m
                .Field("lecture_text") // Поле в виде строки
                .Query("Использование")
            )
        )
    );

    // Возвращаем сырые JSON-данные
    return Results.Ok(response.Documents);
});
app.Run();