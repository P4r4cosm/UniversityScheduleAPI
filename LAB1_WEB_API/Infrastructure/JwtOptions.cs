namespace LAB1_WEB_API.Services;

public class JwtOptions
{
    public string SecretKey { get; set; }
    public int ExpireHours { get; set; }
}