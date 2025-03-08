using AuthService.Application.Dtos;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthService.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly IRefreshTokenService _refreshTokenService;

        public TokenService(UserManager<AppUser> userManager, IConfiguration configuration, IRefreshTokenService refreshTokenService)
        {
            _userManager = userManager;
            _configuration = configuration;
            _refreshTokenService = refreshTokenService;
        }

        public async Task<TokenDto> GenerateTokenAsync(AppUser user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var roles = await _userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new("uid", user.Id.ToString()),
                new(JwtRegisteredClaimNames.Email, user.Email!)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds);

            // Refresh token oluştur
            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user);

            return new TokenDto
            {
                AccessToken = tokenHandler.WriteToken(token),
                RefreshToken = refreshToken.Token,
                Expiration = token.ValidTo
            };
        }
    }
}