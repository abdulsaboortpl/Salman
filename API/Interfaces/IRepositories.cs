using AuthAPI.Models;

namespace AuthAPI.Interfaces;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameOrEmailAsync(string usernameOrEmail);
    Task<IEnumerable<User>> GetAllAsync();
    Task<User> CreateAsync(User user);
    Task<bool> ExistsByEmailOrUsernameAsync(string email, string username);
    Task SaveChangesAsync();
}

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
    Task RevokeAsync(RefreshToken refreshToken);
    Task SaveChangesAsync();
}
