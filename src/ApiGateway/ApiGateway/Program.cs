using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Yarp.ReverseProxy.Configuration;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// Konfig�rasyonlar� al
var jwtConfig = builder.Configuration.GetSection("Jwt");

// JWT Authentication ekle
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtConfig["Issuer"],
            ValidAudience = jwtConfig["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig["Secret"]))
        };
    });

builder.Services.AddAuthorization();  // Yetkilendirme ekle
// YARP Reverse Proxy ekle
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Rate Limiting politikas� ekle
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("RateLimitingPolicy", limiterOptions =>
    {
        limiterOptions.Window = TimeSpan.FromMinutes(1);
        limiterOptions.PermitLimit = 10;                 // 1 dakikada 10 istek
        limiterOptions.QueueLimit = 100;                 // Kuyrukta maksimum 100 istek
        limiterOptions.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

var app = builder.Build();

// Middleware pipeline
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// Belirli Endpoint i�in Rate Limiting
app.MapGet("/api/Products/List", async (HttpContext context) =>
{
    using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
    cts.CancelAfter(TimeSpan.FromSeconds(10));

    await context.Response.WriteAsync("�r�n listesi (Rate Limited Endpoint)", cts.Token);
}).RequireRateLimiting("RateLimitingPolicy")
  .RequireAuthorization();

app.MapGet("/api/Product/Update", async (HttpContext context) =>
{
    await context.Response.WriteAsync("Bu endpoint'e sadece yetkili kullan�c�lar eri�ebilir!");
}).RequireAuthorization();


// Di�er t�m istekleri YARP Reverse Proxy ile y�nlendir
app.MapReverseProxy();

app.Run();
