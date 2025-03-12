using AuthService.Infrastructure.Extensions;
using AuthService.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Logging;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Swagger deste�i
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication ayarlanmas�
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // Token imza do�rulanmas�
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = true, // Token'� olu�turan kimli�i do�rula
            ValidateAudience = true, // Token hedefini do�rula
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true // Token s�resini kontrol et
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AllRoles", policy =>
        policy.RequireRole("User", "Admin"))
    .AddPolicy("MinUser", policy =>
        policy.RequireRole("User", "Admin"));

// Infrastructure katman� DI ayarlar�n� �a��r�yoruz
builder.Services.AddInfrastructureServices(builder.Configuration);

// Ortak loglama yap�land�rmas�.
builder.Services.AddSharedLogging(builder.Configuration);

// Controller eklemesi
builder.Services.AddControllers();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<Guid>>>();
    await RoleSeeder.SeedRolesAsync(roleManager);
}

app.UseSwagger();
app.UseSwaggerUI();

// HTTPS y�nlendirmesi aktif edilir
app.UseHttpsRedirection();

// Authentication (JWT do�rulama) middleware eklenir
app.UseAuthentication();
app.UseAuthorization();

// Controller endpointleri e�lenir.
app.MapControllers();

// Uygulamay� �al��t�r�yoruz.
app.Run();