using AuthService.Infrastructure.Extensions;
using AuthService.Infrastructure.Seed;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Shared.Logging;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Swagger desteði
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// JWT Authentication ayarlanmasý
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opt =>
    {
        opt.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // Token imza doðrulanmasý
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
            ValidateIssuer = true, // Token'ý oluþturan kimliði doðrula
            ValidateAudience = true, // Token hedefini doðrula
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true // Token süresini kontrol et
        };
    });

builder.Services.AddAuthorizationBuilder()
    .AddPolicy("AllRoles", policy =>
        policy.RequireRole("User", "Admin"))
    .AddPolicy("MinUser", policy =>
        policy.RequireRole("User", "Admin"));

// Infrastructure katmaný DI ayarlarýný çaðýrýyoruz
builder.Services.AddInfrastructureServices(builder.Configuration);

// Ortak loglama yapýlandýrmasý.
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

// HTTPS yönlendirmesi aktif edilir
app.UseHttpsRedirection();

// Authentication (JWT doðrulama) middleware eklenir
app.UseAuthentication();
app.UseAuthorization();

// Controller endpointleri eþlenir.
app.MapControllers();

// Uygulamayý çalýþtýrýyoruz.
app.Run();