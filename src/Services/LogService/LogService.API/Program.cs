using LogService.Infrastructure.Extensions;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Serilog'u yapýlandýr
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

//Serilog middleware(Request loglama yapýlacak)
//app.UseSerilogRequestLogging();
//try
//{
//    Log.Information("Log Service Baþlatýldý");
//    app.Run();
//}
//catch (Exception ex)
//{
//    Log.Fatal(ex, "Uygulama beklenmedik þekilde durdu!");
//}
//finally
//{
//    Log.CloseAndFlush();
//}
