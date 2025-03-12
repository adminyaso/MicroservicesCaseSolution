using AuthService.Application.Dtos;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<AppUser> userManager, ITokenService tokenService, IRefreshTokenService refreshTokenService) : ControllerBase
    {
        private readonly UserManager<AppUser> _userManager = userManager;
        private readonly ITokenService _tokenService = tokenService;
        private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            var user = new AppUser
            {
                UserName = registerDto.Email,
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName
            };

            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, loginDto.Password))
            {
                return Unauthorized("Geçersiz kimlik bilgileri");
            }
            // Kullanıcının rolleri kontrol ediliyor.
            // Eğer kullanıcıya daha önce rol atanmadıysa, rol atanıyor
            // **bu kısım register tarafında da olabilir fakat admin register güvenli değil.**
            var roles = await _userManager.GetRolesAsync(user);
            if (!roles.Any())
            {
                if (user.Email.Equals("admin@example.com", StringComparison.OrdinalIgnoreCase))
                {
                    await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    await _userManager.AddToRoleAsync(user, "User");
                }
            }

            var token = await _tokenService.GenerateTokenAsync(user);
            return Ok(token);
        }

        [HttpPost("Refresh")]
        [Authorize(Policy = "MinUser")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            // Kullanıcıyı kimlik doğrulaması için bul
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null)
                return Unauthorized("Geçersiz kullanıcı");

            // Refresh token'ın geçerliliğini kontrol et
            bool isValid = await _refreshTokenService.ValidateRefreshTokenAsync(user.Id, request.RefreshToken);
            if (!isValid)
                return Unauthorized("Geçersiz veya süresi dolmuş refresh token");

            // Yeni token oluştur
            var token = await _tokenService.GenerateTokenAsync(user);
            return Ok(token);
        }

        [HttpPost("Logout")]
        [Authorize(Policy = "MinUser")]
        public async Task<IActionResult> Logout([FromBody] LogoutRequestDto request)
        {
            // Kullanıcı bilgilerini token'dan al
            var userIdClaim = User.FindFirst("uid");
            if (userIdClaim == null)
                return Unauthorized("Kullanıcı bilgisi bulunamadı");

            if (!Guid.TryParse(userIdClaim.Value, out Guid userId))
                return Unauthorized("Geçersiz kullanıcı id");

            // Refresh token'ı iptal et
            await _refreshTokenService.RevokeRefreshTokenAsync(userId, request.RefreshToken);
            return Ok("Çıkış işlemi başarılı.");
        }

        // Sadece admin erişimine açık bir endpoint
        [HttpGet("Tokens")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllTokens([FromServices] AuthDbContext context)
        {
            var tokens = await context.RefreshTokens.ToListAsync();
            return Ok(tokens);
        }
    }
}