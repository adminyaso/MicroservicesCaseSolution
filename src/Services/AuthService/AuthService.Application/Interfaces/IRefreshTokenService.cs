using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces
{
    public interface IRefreshTokenService
    {
        Task<RefreshToken> GenerateRefreshTokenAsync(AppUser user);

        Task<bool> ValidateRefreshTokenAsync(Guid userId, string token);

        Task RevokeRefreshTokenAsync(Guid userId, string token);
    }
}