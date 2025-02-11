using Confluent.Kafka;

namespace Chateq.Core.Domain.Interfaces.Producers;

public interface IKafkaProducer
{
    Task ProduceAsync(string topic, Message<string, string> message);
}