namespace LAB1_WEB_API.Endpoints;

public static class PostgresEndpoints
{
    public static RouteGroupBuilder MapPostgresEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(""); // Опциональный под-префикс
        group.MapGet("/pg_test", (ApplicationContext db) =>
                db.Students.ToList())
            .WithName("GetUsers")
            .WithTags("PostgreSQL");
        return group;
    }
}