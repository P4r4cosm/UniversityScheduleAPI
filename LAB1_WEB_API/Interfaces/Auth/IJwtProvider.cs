namespace LAB1_WEB_API.Services;

public interface IJwtProvider
{
    public string GenerateToken(User user);
}