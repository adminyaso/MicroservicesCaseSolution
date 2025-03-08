using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public static class LoggingExtensions
    {
        /// Konsola basit loglama yapan extension metodu.
        /// Bu metod, merkezi loglama entegrasyonuyla değiştirilecek.
        public static void LogMessage(this string message, string level)
        {
            // Basit format: [LEVEL] Timestamp - Mesaj
            Console.WriteLine($"[{level}] {DateTime.UtcNow:O} - {message}");
        }
    }
}
