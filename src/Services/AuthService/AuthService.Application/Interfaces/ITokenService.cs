using AuthService.Application.Dtos;
using AuthService.Domain.Entities;

namespace AuthService.Application.Interfaces
{
    public interface ITokenService
    {
        Task<TokenDto> GenerateTokenAsync(AppUser user);
    }
}