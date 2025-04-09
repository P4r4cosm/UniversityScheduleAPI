using StackExchange.Redis;
namespace LAB1_WEB_API.Endpoints;

public static class RedisEndpoints
{
    public static RouteGroupBuilder MapRedisEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(""); // Опциональный под-префикс
        group.MapGet("/redis_test", async (IDatabase redis) =>
            {
                var hashEntries = await redis.HashGetAllAsync("student:1");
                var result = hashEntries.ToDictionary(
                    entry => entry.Name.ToString(),
                    entry => entry.Value.ToString()
                );
                return result;
            })
            .WithName("GetRedisData")
            .WithTags("Redis");
        return group;
    }
}