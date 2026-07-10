namespace AuthAPI.Models;

/// <summary>
/// Persisted refresh token for obtaining new access tokens without re-login.
/// </summary>
public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? RevokedAt { get; set; }
    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => RevokedAt == null && !IsExpired;

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
