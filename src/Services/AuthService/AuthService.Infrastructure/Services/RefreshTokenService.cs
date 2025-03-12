using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace AuthService.Infrastructure.Services
{
    public class RefreshTokenService(AuthDbContext context) : IRefreshTokenService
    {
        private readonly AuthDbContext _context = context;

        public async Task<RefreshToken> GenerateRefreshTokenAsync(AppUser user)
        {
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = GenerateSecureToken(),
                Created = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(10), // 10 dakika geçerli
                IsRevoked = false
            };

            _context.Add(refreshToken);
            await _context.SaveChangesAsync();

            return refreshToken;
        }

        private static string GenerateSecureToken()
        {
            // Güvenli rastgele bir token oluşturmak için
            using var rng = RandomNumberGenerator.Create();
            var tokenData = new byte[32];
            rng.GetBytes(tokenData);
            return Convert.ToBase64String(tokenData);
        }

        public async Task<bool> ValidateRefreshTokenAsync(Guid userId, string token)
        {
            var refreshToken = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token && !rt.IsRevoked);

            if (refreshToken == null)
                return false;

            return refreshToken.Expires > DateTime.UtcNow;
        }

        public async Task RevokeRefreshTokenAsync(Guid userId, string token)
        {
            var refreshToken = await _context.Set<RefreshToken>()
                .FirstOrDefaultAsync(rt => rt.UserId == userId && rt.Token == token);

            if (refreshToken != null)
            {
                refreshToken.IsRevoked = true;
                await _context.SaveChangesAsync();
            }
        }
    }
}