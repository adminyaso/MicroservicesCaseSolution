using LogService.Application.Interfaces;
using LogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogService.Infrastructure.Extensions
{
    public static class SerilogExtensions
    {
        /// Serilog yapılandırmasını DI container'a ekleme.
        public static IServiceCollection AddSerilogLogging(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LogDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Serilog yapılandırmasını oluşturuyoruz
            Log.Logger = new LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.Console() // Konsola log yazdırma
                                   // Dosyaya, Seq'e veya Elasticsearch'e yazdırma eklenecek.
                .CreateLogger();

            // DI container'a Serilog'u ekleme
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.ClearProviders();
                loggingBuilder.AddSerilog(dispose: true);
            });
            // LogService implementasyonunu DI container'a ekleme
            services.AddScoped<ILogService, Services.LogService>();

            return services;
        }
    }
}
