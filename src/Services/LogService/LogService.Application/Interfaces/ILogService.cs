using LogService.Application.DTOs;

namespace LogService.Application.Interfaces
{
    public interface ILogService
    {
        Task SaveLogAsync(LogDto logDto);

        Task<IEnumerable<LogDto>> GetLogsAsync();

        Task<LogReportDto> GetLogReportAsync(LogReportFilterDto filter);
    }
}