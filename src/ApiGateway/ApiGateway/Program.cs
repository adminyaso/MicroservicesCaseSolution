using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// JWT yap�land�rmas� (API Gateway'de merkezi kimlik do�rulama i�in)
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

// Rate Limiting i�in �zel politika tan�mlayal�m
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("RateLimitingPolicy", context =>
    {
        // Client IP baz�nda limit
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, partition =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,// �rne�in, 10 istek/pencere
                Window = TimeSpan.FromMinutes(1),// 1 dakikal�k pencere
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
    options.RejectionStatusCode = 429;// Rate limit a��ld���nda 429 d�ner
});

// YARP Reverse Proxy ve di�er servis konfig�rasyonlar�
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Global middleware yerine, sadece belirli endpoint�lerde kullanaca��z
// YARP'nin Reverse Proxy'si t�m istekleri al�rken,
// �r�n listeleme i�in ayr� bir endpoint tan�mlay�p bu endpoint�e �zel rate limiting uygulanabilir.

app.MapGet("/api/products/list", async (HttpContext context) =>
{
    // �lgili �r�n mikroservisine prox.
    await context.Response.WriteAsync("�r�n listesi (Rate Limited Endpoint)");
})
.RequireRateLimiting("RateLimitingPolicy");

// Di�er reverse proxy rotalar�n� API Gateway �zerinden yap�land�rma
app.MapReverseProxy();

app.Run();
