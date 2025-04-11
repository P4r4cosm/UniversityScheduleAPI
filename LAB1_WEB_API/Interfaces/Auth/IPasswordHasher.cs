namespace LAB1_WEB_API.Services;

public interface IPasswordHasher
{
    string GenerateHash(string password);
    
    bool VerifyHash(string password, string hash);
}