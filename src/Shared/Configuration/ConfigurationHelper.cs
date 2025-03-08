using Microsoft.Extensions.Configuration;
using System.IO;

namespace Shared.Configuration
{
    public static class ConfigurationHelper
    {
        /// Uygulamanın konfigürasyon ayarlarını okur.
        /// appsettings.json dosyasını ve ortam değişkenlerini kullanır.
        /// return IConfiguration instance'ı
        public static IConfiguration GetConfiguration()
        {
            var builder = new ConfigurationBuilder()
                // Projenin çalışma dizinini temel alarak konfigürasyon dosyalarını yükler.
                .SetBasePath(Directory.GetCurrentDirectory())
                // appsettings.json dosyasını yükler; dosya opsiyoneldir, değişiklikler reload edilebilir.
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                // Ortam değişkenlerini de konfigürasyona ekler.
                .AddEnvironmentVariables();

            return builder.Build();
        }
    }
}
