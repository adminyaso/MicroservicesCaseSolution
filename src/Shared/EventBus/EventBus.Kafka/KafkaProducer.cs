// Shared/EventBus/EventBus.Kafka/KafkaProducer.cs
using Confluent.Kafka;
using EventBus.Kafka;
using System.Text.Json;

namespace Shared.EventBus
{
    public class KafkaProducer : IKafkaProducer
    {
        private readonly IProducer<Null, string> _producer;
        private readonly string _topic;

        public KafkaProducer(string bootstrapServers, string topic = "events-topic")
        {
            _topic = topic;
            var config = new ProducerConfig { BootstrapServers = bootstrapServers };
            _producer = new ProducerBuilder<Null, string>(config).Build();
        }

        public async Task PublishAsync<TEvent>(TEvent @event) where TEvent : class
        {
            var eventData = JsonSerializer.Serialize(@event);
            await _producer.ProduceAsync(_topic, new Message<Null, string> { Value = eventData });
        }
    }
}
