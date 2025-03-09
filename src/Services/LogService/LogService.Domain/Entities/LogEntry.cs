using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LogService.Domain.Entities
{
    public class LogEntry
    {
        public Guid Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string LogLevel { get; set; } = string.Empty; // INFO, WARNING, ERROR, CRITICAL
        public string Message { get; set; } = string.Empty;
        public string? Exception { get; set; } // Hata varsa detayları
        public string Source { get; set; } = string.Empty; // Log kaynağı
    }
}
