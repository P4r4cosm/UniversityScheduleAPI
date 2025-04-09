using Elastic.Clients.Elasticsearch.Nodes;
using LAB1_WEB_API.Contracts.Users;
using LAB1_WEB_API.Services;

namespace LAB1_WEB_API.Endpoints;

public static class UserEndpoint
{
    public static IEndpointRouteBuilder MapUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("register", Register);
        app.MapPost("login", Login);
        return app;
    }

    public static async Task<IResult> Register(RegisterUserRequest request,UserServices userServices)
    {
        await userServices.Register(request.Name, request.Password);
        return Results.Ok();
    }
    public static async Task<IResult> Login(LoginUserRequest request,UserServices userServices, HttpContext httpContext)
    {
        var token = await userServices.Login(request.Name, request.Password);
        if (token == null)
        {
            return Results.Unauthorized(); // Возвращаем 401 Unauthorized, если вход не удался
        }
        
        httpContext.Response.Cookies.Append("cookies", token);
        
        return Results.Ok(token); // Возвращаем токен в теле ответа
    }
}