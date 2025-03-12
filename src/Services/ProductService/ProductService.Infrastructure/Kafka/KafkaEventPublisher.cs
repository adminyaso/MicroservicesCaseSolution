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

        public KafkaEventPublisher(IConfiguration configuration)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                //Acks = Acks.All, // Broker'dan tam onay bekler
                MessageTimeoutMs = 1000 // 1000ms sonra timeout verilir
                // Gerekirse ek ayarlar...
            };

            _topic = configuration["Kafka:Topic"] ?? "product-events";

            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync<TEvent>(TEvent @event, CancellationToken cancellationToken = default)
        {
            var message = JsonSerializer.Serialize(@event);
            var kafkaMessage = new Message<Null, string> { Value = message };
            //Log.Error("Publish başlıyor");
            // Publish'e abone mikroservis mesajı alacak mantıklı kayıt olmadığı için devre dışı timeout'a gidiyor.
            //await _producer.ProduceAsync(_topic, kafkaMessage, cancellationToken);
            //Log.Error("Publish bitti");
        }

        public void Dispose()
        {
            _producer.Flush(TimeSpan.FromSeconds(10));
            _producer.Dispose();
        }
    }
}
