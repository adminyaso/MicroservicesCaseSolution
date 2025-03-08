using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// YARP yapýlandýrmasý
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Rate Limiting ekleniyor
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("fixedWindowPolicy", opt =>
    {
        opt.PermitLimit = 100; // dakikada 100 request izin verir
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();

app.UseRateLimiter();

app.MapReverseProxy();

app.Run();
