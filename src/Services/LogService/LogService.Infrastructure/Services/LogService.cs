using LogService.Application.DTOs;
using LogService.Application.Interfaces;
using LogService.Domain.Entities;
using LogService.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Serilog.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogService.Infrastructure.Services
{
    public class LogService : ILogService
    {
        private readonly LogDbContext _context;

        public LogService(LogDbContext context)
        {
            _context = context;
        }

        public async Task SaveLogAsync(LogDto logDto)
        {
            var logEntry = new LogEntry
            {
                Id = logDto.Id == Guid.Empty ? Guid.NewGuid() : logDto.Id,
                Timestamp = logDto.Timestamp,
                LogLevel = logDto.LogLevel,
                Message = logDto.Message,
                Exception = logDto.Exception,
                Source = logDto.Source
            };

            _context.LogEntries.Add(logEntry);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<LogDto>> GetLogsAsync()
        {
            return await _context.LogEntries
                .Select(logentity => new LogDto
                {
                    Id = logentity.Id,
                    Timestamp = logentity.Timestamp,
                    LogLevel = logentity.LogLevel,
                    Message = logentity.Message,
                    Exception = logentity.Exception,
                    Source = logentity.Source
                })
                .ToListAsync();
        }
    }
}
