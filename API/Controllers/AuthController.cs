using System.Security.Claims;
using AuthAPI.DTOs;
using AuthAPI.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers;

/// <summary>
/// Authentication endpoints: register, login, refresh token, and profile.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>Register a new user account.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterDto registerDto)
    {
        var result = await _authService.RegisterAsync(registerDto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Registration successful."));
    }

    /// <summary>Authenticate with username/email and password.</summary>
    [HttpPost("login")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginDto loginDto)
    {
        var result = await _authService.LoginAsync(loginDto);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Login successful."));
    }

    /// <summary>Exchange a valid refresh token for a new access token.</summary>
    [HttpPost("refresh")]
    [AllowAnonymous]
    [ProducesResponseType(typeof(ApiResponse<AuthResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
    {
        var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);
        return Ok(ApiResponse<AuthResponseDto>.Ok(result, "Token refreshed successfully."));
    }

    /// <summary>Get the currently authenticated user's profile.</summary>
    [HttpGet("profile")]
    [Authorize]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<UserDto>>> GetProfile()
    {
        var userId = GetCurrentUserId();
        var profile = await _authService.GetProfileAsync(userId);
        return Ok(ApiResponse<UserDto>.Ok(profile));
    }

    private int GetCurrentUserId()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new UnauthorizedAccessException("Invalid token.");

        if (!int.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid token.");

        return userId;
    }
}
