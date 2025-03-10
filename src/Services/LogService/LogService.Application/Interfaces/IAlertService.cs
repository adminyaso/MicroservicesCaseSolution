using LogService.Domain.Entities;

namespace LogService.Application.Interfaces
{
    public interface IAlertService
    {
        Task SendAlertAsync(LogEntry logEntry);
    }
}