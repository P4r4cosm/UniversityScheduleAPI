using LAB1_WEB_API.Repositories;

namespace LAB1_WEB_API.Services;

public class UserServices
{
    private readonly IPasswordHasher _hasher;
    private readonly UserRepository _userRepository;
    private readonly IJwtProvider _jwtProvider;

    public UserServices(IPasswordHasher hasher, UserRepository userRepository, IJwtProvider jwtProvider)
    {
        _hasher = hasher;
        _userRepository = userRepository;
        _jwtProvider = jwtProvider;
    }

    public async Task Register(string username, string password)
    {
        var hashedPassword = _hasher.GenerateHash(password);

        var user = new User { Name = username, PasswordHash = hashedPassword };

        await _userRepository.Create(user);
    }

    public async Task<string> Login(string username, string password)
    {
        var user = await _userRepository.GetByName(username);
        //если пользователь не найден возвращаем null
        if (user == null) return null;
        if (!_hasher.VerifyHash(password, user.PasswordHash)) return null;
        return _jwtProvider.GenerateToken(user);
    }
}