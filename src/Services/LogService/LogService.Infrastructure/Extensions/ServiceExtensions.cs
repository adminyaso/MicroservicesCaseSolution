using LogService.Application.Interfaces;
using LogService.Infrastructure.Data;
using LogService.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LogService.Infrastructure.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddLogInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<LogDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<ILogService, Services.LogService>();
            services.AddScoped<IAlertService, AlertService>();

            return services;
        }
    }
}