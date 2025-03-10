using LogService.Application.DTOs;
using LogService.Application.Interfaces;
using LogService.Domain.Entities;
using LogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace LogService.Infrastructure.Services
{
    public class LogService : ILogService
    {
        private readonly LogDbContext _context;
        private readonly IAlertService _alertService;

        public LogService(LogDbContext context, IAlertService alertService)
        {
            _context = context;
            _alertService = alertService;
        }

        public async Task SaveLogAsync(LogDto logDto)
        {
            // Enrichment: Eksik alanları doldur
            if (string.IsNullOrWhiteSpace(logDto.EnvironmentName))
                logDto.EnvironmentName = "Logservis";
            if (string.IsNullOrWhiteSpace(logDto.MachineName))
                logDto.MachineName = Environment.MachineName;

            var logEntry = new LogEntry
            {
                Timestamp = logDto.Timestamp == default ? DateTime.UtcNow : logDto.Timestamp,
                Message = logDto.Message,
                Level = logDto.Level,
                Exception = logDto.Exception,
                Properties = logDto.Properties,
                EnvironmentName = logDto.EnvironmentName,
                MachineName = logDto.MachineName,
                ExceptionDetail = logDto.ExceptionDetail,
                Path = logDto.Path,
                RequestPath = logDto.RequestPath,
                SourceContext = logDto.SourceContext
            };

            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync();

            // Alerting: Eğer log seviyesi ERROR veya FATAL ise alert gönder
            if (logEntry.Level.Equals("ERROR", StringComparison.OrdinalIgnoreCase) ||
                logEntry.Level.Equals("FATAL", StringComparison.OrdinalIgnoreCase))
            {
                await _alertService.SendAlertAsync(logEntry);
            }
        }

        public async Task<IEnumerable<LogDto>> GetLogsAsync()
        {
            return await _context.LogEntries
                .Select(logentity => new LogDto
                {
                    Id = logentity.Id,
                    Timestamp = logentity.Timestamp,
                    Message = logentity.Message,
                    Level = logentity.Level,
                    Exception = logentity.Exception,
                    Properties = logentity.Properties,
                    EnvironmentName = logentity.EnvironmentName,
                    MachineName = logentity.MachineName,
                    ExceptionDetail = logentity.ExceptionDetail,
                    Path = logentity.Path,
                    RequestPath = logentity.RequestPath,
                    SourceContext = logentity.SourceContext
                })
                .ToListAsync();
        }

        public async Task<LogReportDto> GetLogReportAsync(LogReportFilterDto filter)
        {
            var query = _context.LogEntries.AsQueryable();

            if (filter.StartDate.HasValue)
                query = query.Where(le => le.Timestamp >= filter.StartDate.Value);
            if (filter.EndDate.HasValue)
                query = query.Where(le => le.Timestamp <= filter.EndDate.Value);

            var totalLogs = await query.CountAsync();
            var infoCount = await query.CountAsync(le => le.Level.Equals("INFO", StringComparison.OrdinalIgnoreCase));
            var warningCount = await query.CountAsync(le => le.Level.Equals("WARNING", StringComparison.OrdinalIgnoreCase));
            var errorCount = await query.CountAsync(le => le.Level.Equals("ERROR", StringComparison.OrdinalIgnoreCase));
            var criticalCount = await query.CountAsync(le => le.Level.Equals("CRITICAL", StringComparison.OrdinalIgnoreCase));

            return new LogReportDto
            {
                TotalLogs = totalLogs,
                InfoCount = infoCount,
                WarningCount = warningCount,
                ErrorCount = errorCount,
                CriticalCount = criticalCount,
                ReportGeneratedAt = DateTime.UtcNow
            };
        }
    }
}