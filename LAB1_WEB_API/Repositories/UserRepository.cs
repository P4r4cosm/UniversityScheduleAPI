using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LAB1_WEB_API.Repositories;

public class UserRepository
{
    private readonly ApplicationContext _context;

    public UserRepository(ApplicationContext dbContext)
        => _context = dbContext;

    public async Task Create(User user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task<User> GetByName(string name)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Name == name);
    }
}