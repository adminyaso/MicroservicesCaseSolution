using LogService.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog'u yap�land�r
builder.Services.AddSerilogLogging(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

app.Run();

//Serilog middleware(Request loglama yap�lacak)
//app.UseSerilogRequestLogging();
//try
//{
//    Log.Information("Log Service Ba�lat�ld�");
//    app.Run();
//}
//catch (Exception ex)
//{
//    Log.Fatal(ex, "Uygulama beklenmedik �ekilde durdu!");
//}
//finally
//{
//    Log.CloseAndFlush();
//}
