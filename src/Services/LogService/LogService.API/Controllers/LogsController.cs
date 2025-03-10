using LogService.Application.DTOs;
using LogService.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace LogService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        // Test veya manuel log kaydı
        [HttpPost("Save")]
        public async Task<IActionResult> SaveLog([FromBody] LogDto logDto)
        {
            await _logService.SaveLogAsync(logDto);
            return Ok("Log kaydedildi.");
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _logService.GetLogsAsync();
            return Ok(logs);
        }

        // Raporlama zaman filtreli(Seq' arayüzüde yapıyor...)
        [HttpGet("Report")]
        public async Task<IActionResult> GetLogReport([FromQuery] LogReportFilterDto filter)
        {
            var report = await _logService.GetLogReportAsync(filter);
            return Ok(report);
        }
    }
}