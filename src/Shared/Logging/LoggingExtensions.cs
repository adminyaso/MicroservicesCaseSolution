using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Exceptions;
using Serilog.Extensions.Hosting;
using Serilog.Formatting.Json;
using Serilog.Sinks.MSSqlServer;
using System.Collections.ObjectModel;
using System.Data;

namespace Shared.Logging
{
    public static class LoggingExtensions
    {
        public static IServiceCollection AddSharedLogging(this IServiceCollection services, IConfiguration configuration)
        {
            // MSSQL için connection string'i configuration üzerinden alıyoruz.
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // ColumnOptions'ı AdditionalColumns koleksiyonu ile yapılandırıyoruz.
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.AdditionalColumns = new Collection<SqlColumn>
            {
                new SqlColumn { ColumnName = "EnvironmentName", DataType = SqlDbType.NVarChar, AllowNull = true },
                new SqlColumn { ColumnName = "MachineName", DataType = SqlDbType.NVarChar, AllowNull = true },
                new SqlColumn { ColumnName = "ExceptionDetail", DataType = SqlDbType.NVarChar, AllowNull = true },
                new SqlColumn { ColumnName = "Path", DataType = SqlDbType.NVarChar, AllowNull = true },
                new SqlColumn { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, AllowNull = true },
                new SqlColumn { ColumnName = "SourceContext", DataType = SqlDbType.NVarChar, AllowNull = true }
            };

            // Serilog yapılandırmasını oluşturuyoruz.
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .Enrich.WithEnvironmentName()
                .Enrich.WithMachineName()
                .Enrich.WithExceptionDetails()
                .WriteTo.Console(new JsonFormatter())  // Konsola JSON formatında log
                .WriteTo.Seq(configuration["Seq:ServerUrl"] ?? "http://localhost:5341")
                .WriteTo.MSSqlServer(
                    connectionString: connectionString,
                    sinkOptions: new MSSqlServerSinkOptions { TableName = "LogEntry", AutoCreateSqlTable = true },
                    columnOptions: columnOptions)
                .CreateLogger();

            // DiagnosticContext için ILogger'ı DI'ya ekleyin
            services.AddSingleton<Serilog.ILogger>(Log.Logger);
            services.AddSingleton<DiagnosticContext>();

            // Loglama sağlayıcısını DI container'a ekleyelim.
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });

            // Loglama yapılandırmasının başladığını loglayalım.
            Log.Information("Shared Logging yapılandırması tamamlandı.");

            return services;
        }

        // Middleware eklemesi
        public static IApplicationBuilder UseSharedRequestLogging(this IApplicationBuilder app)
        {
            // Serilog request logging middleware'ini ekler.
            return app.UseSerilogRequestLogging();
        }
    }
}