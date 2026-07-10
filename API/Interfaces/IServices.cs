using AuthAPI.DTOs;
using AuthAPI.Models;

namespace AuthAPI.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken);
    Task<UserDto> GetProfileAsync(int userId);
}

public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshToken GenerateRefreshToken(int userId);
    int? ValidateAccessTokenUserId(string token);
}

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
}
