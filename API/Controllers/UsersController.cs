using AuthAPI.DTOs;
using AuthAPI.Interfaces;
using AuthAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthAPI.Controllers;

/// <summary>
/// User management endpoints (requires authentication).
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>Get all registered users. Admin and User roles can access.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(ApiResponse<IEnumerable<UserDto>>), StatusCodes.Status200OK)]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(users));
    }

    /// <summary>Admin-only endpoint example for role-based authorization.</summary>
    [HttpGet("admin-only")]
    [Authorize(Roles = Roles.Admin)]
    [ProducesResponseType(typeof(ApiResponse<string>), StatusCodes.Status200OK)]
    public ActionResult<ApiResponse<string>> AdminOnly()
    {
        return Ok(ApiResponse<string>.Ok("You have Admin access."));
    }
}
