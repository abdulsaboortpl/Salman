namespace AuthAPI.Models;

/// <summary>
/// Core user entity stored in SQL Server via Entity Framework Core.
/// </summary>
public class User
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public string Role { get; set; } = Roles.User;
    public bool IsActive { get; set; } = true;
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}

/// <summary>
/// Application role constants used for JWT claims and authorization policies.
/// </summary>
public static class Roles
{
    public const string Admin = "Admin";
    public const string User = "User";
}
