using Chateq.Core.Domain.Constants;
using Chateq.Core.Domain.Interfaces.Producers;
using Chateq.Core.Domain.Options;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Chateq.Infrastructure.Producers;

public class KafkaProducer : IKafkaProducer
{
    private readonly IProducer<string, string> _producer;
    private readonly ILogger<KafkaProducer> _logger;

    public KafkaProducer(IOptions<KafkaOption> options, ILogger<KafkaProducer> logger)
    {
        var kafkaSettings = options.Value;
        var config = new ConsumerConfig
        {
            GroupId = GroupKafka.Message,
            BootstrapServers = kafkaSettings.Url,
            AutoOffsetReset = AutoOffsetReset.Earliest,
        };

        _producer = new ProducerBuilder<string, string>(config).Build();
        _logger = logger;
    }

    public async Task ProduceAsync(string topic, Message<string, string> message)
    {
        try
        {
            await _producer.ProduceAsync(topic, message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"System got error during sending message on topic: {topic}");
            throw;
        }
    }
}