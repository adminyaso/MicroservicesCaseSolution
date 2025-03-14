using LogService.Infrastructure.Extensions;
using LogService.Infrastructure.Kafka;
using Serilog;
using Shared.Logging;

var builder = WebApplication.CreateBuilder(args);

// Ortak loglama yapılandırması.
builder.Services.AddSharedLogging(builder.Configuration);

// DI Kayıtları
builder.Services.AddLogInfrastructureServices(builder.Configuration);
builder.Services.AddHostedService<KafkaEventConsumer>();

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
    Log.Information("Log Service Başlatıldı");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Uygulama beklenmedik şekilde durdu!");
}
finally
{
    Log.CloseAndFlush();
}