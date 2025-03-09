using LogService.Application.DTOs;
using LogService.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LogService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Roles = "Admin")] // Sadece yetkili kişiler loglara erişebilsin
    public class LogController : ControllerBase
    {
        private readonly ILogService _logService;

        public LogController(ILogService logService)
        {
            _logService = logService;
        }

        [HttpPost("Save")]
        public async Task<IActionResult> SaveLog([FromBody] LogDto logDto)
        {
            await _logService.SaveLogAsync(logDto);
            return Ok("Log kaydedildi");
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _logService.GetLogsAsync();
            return Ok(logs);
        }
    }
}
