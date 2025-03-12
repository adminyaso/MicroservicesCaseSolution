using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// JWT yapýlandýrmasý (API Gateway'de merkezi kimlik doðrulama için)
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

// Rate Limiting için özel politika tanýmlayalým
builder.Services.AddRateLimiter(options =>
{
    options.AddPolicy("RateLimitingPolicy", context =>
    {
        // Client IP bazýnda limit
        var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        return RateLimitPartition.GetFixedWindowLimiter(clientIp, partition =>
            new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,// Örneðin, 10 istek/pencere
                Window = TimeSpan.FromMinutes(1),// 1 dakikalýk pencere
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            });
    });
    options.RejectionStatusCode = 429;// Rate limit aþýldýðýnda 429 döner
});

// YARP Reverse Proxy ve diðer servis konfigürasyonlarý
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

var app = builder.Build();

// Global middleware yerine, sadece belirli endpoint’lerde kullanacaðýz
// YARP'nin Reverse Proxy'si tüm istekleri alýrken,
// ürün listeleme için ayrý bir endpoint tanýmlayýp bu endpoint’e özel rate limiting uygulanabilir.

app.MapGet("/api/products/list", async (HttpContext context) =>
{
    // Ýlgili ürün mikroservisine prox.
    await context.Response.WriteAsync("Ürün listesi (Rate Limited Endpoint)");
})
.RequireRateLimiting("RateLimitingPolicy");

// Diðer reverse proxy rotalarýný API Gateway üzerinden yapýlandýrma
app.MapReverseProxy();

app.Run();
