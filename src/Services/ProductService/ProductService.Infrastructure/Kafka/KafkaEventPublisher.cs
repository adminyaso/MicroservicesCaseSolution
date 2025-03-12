using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using ProductService.Application.Interfaces;
using Serilog;
using System.Text.Json;

namespace ProductService.Infrastructure.Kafka
{
    public class KafkaEventPublisher : IEventPublisher, IDisposable
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;
        private bool _disposed;

        public KafkaEventPublisher(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                //Acks = Acks.All, // Broker'dan tam onay bekler
/*                MessageTimeoutMs = 1000*/ // 1000ms sonra timeout verilir
                // Gerekirse ek ayarlar...
            };

            _topic = configuration["Kafka:Topic"] ?? "product-events";

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        {
            // using ile kullanılmalı
            var message = JsonSerializer.Serialize(@event);
            var kafkaMessage = new Message<Null, string> { Value = message };
            Log.Information("Publish başlıyor");
            //Publish'e abone mikroservis mesajı alacak mantıklı kayıt olmadığı için devre dışı timeout'a gidiyor.
            await _producer.ProduceAsync(_topic, kafkaMessage, cancellationToken);
            Log.Information("Publish bitti");
        }

        // Dispose pattern'in asıl uygulaması
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Managed kaynakları serbest bırakın
                    _producer.Flush(TimeSpan.FromSeconds(10));
                    _producer.Dispose();
                }
                // Eğer unmanaged kaynaklar varsa burada temizlenir.

                _disposed = true;
            }
        }

        // IDisposable.Dispose() metodu
        public void Dispose()
        {
            Dispose(true);
            // Finalizer'ın çağrılmasını engeller
            GC.SuppressFinalize(this);
        }

        // Finalizer metot, eğer unmanaged varsa çağrılır.
        ~KafkaEventPublisher()
        {
            Dispose(false);
        }
    }
}
