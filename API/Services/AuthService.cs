using AuthAPI.DTOs;
using AuthAPI.Helpers;
using AuthAPI.Interfaces;
using AuthAPI.Models;
using AutoMapper;
using Microsoft.Extensions.Options;

namespace AuthAPI.Services;

/// <summary>
/// Handles registration, login, token refresh, and profile retrieval.
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly JwtSettings _jwtSettings;

    public AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        ITokenService tokenService,
        IMapper mapper,
        IOptions<JwtSettings> jwtSettings)
    {
        _userRepository = userRepository;
        _refreshTokenRepository = refreshTokenRepository;
        _tokenService = tokenService;
        _mapper = mapper;
        _jwtSettings = jwtSettings.Value;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        if (await _userRepository.ExistsByEmailOrUsernameAsync(registerDto.Email, registerDto.Username))
            throw new InvalidOperationException("Email or username is already registered.");

        var user = _mapper.Map<User>(registerDto);
        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        await _userRepository.CreateAsync(user);
        await _userRepository.SaveChangesAsync();

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByUsernameOrEmailAsync(loginDto.UsernameOrEmail);

        if (user is null || !user.IsActive)
            throw new UnauthorizedAccessException("Invalid credentials.");

        if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials.");

        return await BuildAuthResponseAsync(user);
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken)
    {
        var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);

        if (storedToken is null || !storedToken.IsActive)
            throw new UnauthorizedAccessException("Invalid or expired refresh token.");

        if (!storedToken.User.IsActive)
            throw new UnauthorizedAccessException("User account is inactive.");

        await _refreshTokenRepository.RevokeAsync(storedToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return await BuildAuthResponseAsync(storedToken.User);
    }

    public async Task<UserDto> GetProfileAsync(int userId)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        return _mapper.Map<UserDto>(user);
    }

    private async Task<AuthResponseDto> BuildAuthResponseAsync(User user)
    {
        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

        await _refreshTokenRepository.CreateAsync(refreshToken);
        await _refreshTokenRepository.SaveChangesAsync();

        return new AuthResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken.Token,
            AccessTokenExpiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.AccessTokenExpirationMinutes),
            User = _mapper.Map<UserDto>(user)
        };
    }
}
