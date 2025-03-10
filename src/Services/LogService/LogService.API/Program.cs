using LogService.Infrastructure.Extensions;
using Serilog;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

// Ortak loglama yap�land�rmas�.
builder.Services.AddSharedLogging(builder.Configuration);

// DI Kay�tlar�
builder.Services.AddLogInfrastructureServices(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// Ortak request loglama middleware'i
app.UseSharedRequestLogging();

app.MapControllers();

try
{
    Log.Information("Log Service Ba�lat�ld�");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama beklenmedik �ekilde durdu!");
}
finally
{
    Log.CloseAndFlush();
}