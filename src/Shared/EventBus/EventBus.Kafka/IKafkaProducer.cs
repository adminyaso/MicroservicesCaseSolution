using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Kafka
{
    public interface IKafkaProducer
    {
        Task PublishAsync<TEvent>(TEvent @event) where TEvent : class;
    }
}
