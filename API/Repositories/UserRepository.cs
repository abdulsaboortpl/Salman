using AuthAPI.Data;
using AuthAPI.Interfaces;
using AuthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Repositories;

/// <summary>
/// Data access layer for User entities.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(int id) =>
        await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Id == id);

    public async Task<User?> GetByUsernameAsync(string username) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

    public async Task<User?> GetByEmailAsync(string email) =>
        await _context.Users.FirstOrDefaultAsync(u => u.Email == email);

    public async Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail) =>
        await _context.Users.FirstOrDefaultAsync(u =>
            u.Username == usernameOrEmail || u.Email == usernameOrEmail);

    public async Task<IEnumerable<User>> GetAllAsync() =>
        await _context.Users.AsNoTracking().OrderBy(u => u.FullName).ToListAsync();

    public async Task<User> CreateAsync(User user)
    {
        await _context.Users.AddAsync(user);
        return user;
    }

    public async Task<bool> ExistsByEmailOrUsernameAsync(string email, string username) =>
        await _context.Users.AnyAsync(u => u.Email == email || u.Username == username);

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
