using LogService.Application.Interfaces;
using LogService.Domain.Entities;

namespace LogService.Infrastructure.Services
{
    public class AlertService : IAlertService
    {
        public Task SendAlertAsync(LogEntry logEntry)
        {
            Console.WriteLine($"ALERT: {logEntry.Level} at {logEntry.Timestamp}: {logEntry.Message}");
            // Burada mail, SMS veya Slack bildirimi yapılabilir.
            return Task.CompletedTask;
        }
    }
}