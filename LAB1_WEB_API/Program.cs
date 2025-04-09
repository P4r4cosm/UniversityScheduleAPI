using LAB1_WEB_API;
using LAB1_WEB_API.Endpoints;
using LAB1_WEB_API.Repositories;
using LAB1_WEB_API.Services;
using Microsoft.AspNetCore.CookiePolicy;

var builder = WebApplication.CreateBuilder(args);

// Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("JwtOptions"));
//добавляем базы
builder.Services.AddPostgres(builder.Configuration);
builder.Services.AddNeo4j(builder.Configuration);
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddMongoDb(builder.Configuration);
builder.Services.AddElastic(builder.Configuration);
//Сервисы для JWT
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<IJwtProvider, JwtProvider>();
builder.Services.AddScoped<UserServices>();
builder.Services.AddScoped<IPasswordHasher, PasswordHasher>();

builder.Services.AddApiAuthentication(builder.Configuration.GetSection("JwtOptions")); // схема аутентификации - с помощью jwt-токенов.
builder.Services.AddAuthorization();
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
app.MapUserEndpoint();

//регистрируем все базы данных
var group = app.MapGroup("/api/v1").RequireAuthorization();
group.AddDataBaseEndPoints();
app.Run();