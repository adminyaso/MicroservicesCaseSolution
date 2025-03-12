using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace LogService.Infrastructure.Kafka
{
    public class KafkaEventConsumer : BackgroundService
    {
        private readonly ILogger<KafkaEventConsumer> _logger;
        private readonly ConsumerConfig _consumerConfig;
        private readonly string _topic;

        public KafkaEventConsumer(ILogger<KafkaEventConsumer> logger, IConfiguration configuration)
        {
            _logger = logger;
            _topic = configuration["Kafka:Topic"] ?? "product-events";
            _consumerConfig = new ConsumerConfig
            {
                BootstrapServers = configuration["Kafka:BootstrapServers"],
                GroupId = "logservice-consumer-group", // Her servis örneği aynı
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true
            };
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(() =>
            {
                using var consumer = new ConsumerBuilder<Ignore, string>(_consumerConfig).Build();
                consumer.Subscribe(_topic);

                try
                {
                    while (!stoppingToken.IsCancellationRequested)
                    {
                        try
                        {
                            var result = consumer.Consume(stoppingToken);
                            _logger.LogInformation("Kafka mesaj alındııı: {Message}", result.Message.Value);
                        }
                        catch (ConsumeException ex)
                        {
                            _logger.LogError(ex, "Kafka mesajı alınırken hata oluştu.");
                        }
                    }
                }
                catch (OperationCanceledException ex)
                {
                    _logger.LogError(ex, "Kafka işleminde uygulama kapandı.");
                }
                finally
                {
                    consumer.Close();
                }
            }, stoppingToken);
        }
    }
}
