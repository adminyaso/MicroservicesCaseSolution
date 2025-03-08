using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilites
{
    public static class CommonHelpers
    {
        /// Basit bilgi loglama metodu.
        /// Gerçek projede bu metod, merkezi loglama (ör. Serilog) ile entegre edilebilir.
        public static void LogInfo(string message)
        {
            // Bu örnekte konsola log yazılıyor.
            Console.WriteLine($"INFO: {message}");
        }

        /// Hata mesajını formatlayan basit yardımcı metod.
        public static string FormatErrorMessage(string error)
        {
            return $"[ERROR] {DateTime.UtcNow}: {error}";
        }
    }
}
