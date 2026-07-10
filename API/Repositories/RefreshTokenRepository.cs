using AuthAPI.Data;
using AuthAPI.Interfaces;
using AuthAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthAPI.Repositories;

/// <summary>
/// Data access layer for refresh token persistence and revocation.
/// </summary>
public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token) =>
        await _context.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == token);

    public async Task<RefreshToken> CreateAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
        return refreshToken;
    }

    public Task RevokeAsync(RefreshToken refreshToken)
    {
        refreshToken.RevokedAt = DateTime.UtcNow;
        _context.RefreshTokens.Update(refreshToken);
        return Task.CompletedTask;
    }

    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}
