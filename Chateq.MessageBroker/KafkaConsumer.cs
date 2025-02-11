using System.Text.Json;
using Chateq.Core.Domain;
using Chateq.Core.Domain.Constants;
using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Models;
using Chateq.Core.Domain.Options;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Chateq.MessageBroker;

public class KafkaConsumer : BackgroundService
{
    private readonly KafkaOption _kafkaOption;
    private readonly ILogger<KafkaConsumer> _logger;
    private readonly IDbContextFactory<ChateqDbContext> _dbContextFactory;

    public KafkaConsumer(IOptions<KafkaOption> kafkaOption, ILogger<KafkaConsumer> logger,
        IDbContextFactory<ChateqDbContext> dbContextFactory)
    {
        _kafkaOption = kafkaOption.Value;
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await ConsumeAsync(TopicKafka.Message, stoppingToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while consuming messages.");
        }
    }

    private async Task ConsumeAsync(string topic, CancellationToken stoppingToken)
    {
        var config = CreateConsumerConfig();
        using var consumer = new ConsumerBuilder<string, string>(config).Build();

        consumer.Subscribe(topic);
        _logger.LogInformation($"Subscribed to topic: {topic}");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                var consumeResult = consumer.Consume(stoppingToken);
                await ProcessMessageAsync(consumeResult.Message.Value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the message.");
                await Task.Delay(1000, stoppingToken);
            }
        }
    }

    private ConsumerConfig CreateConsumerConfig()
    {
        return new ConsumerConfig
        {
            GroupId = GroupKafka.Message,
            BootstrapServers = _kafkaOption.Url,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
    }

    private async Task ProcessMessageAsync(string value)
    {
        var messageDto = JsonSerializer.Deserialize<MessageDto>(value);

        var message = CreateMessage(messageDto!);
        await SaveMessageToDatabaseAsync(message);
    }

    private static Message CreateMessage(MessageDto messageDto)
    {
        return new Message
        {
            Id = messageDto.Id,
            SenderId = messageDto.SenderId,
            CreatedAt = messageDto.CreatedAt,
            MessageText = messageDto.MessageText,
            ChatId = messageDto.ChatId,
        };
    }

    private async Task SaveMessageToDatabaseAsync(Message message)
    {
        try
        {
            var dbContext = await _dbContextFactory.CreateDbContextAsync();
            await dbContext.Messages.AddAsync(message);
            await dbContext.SaveChangesAsync();
            _logger.LogInformation($"Message with id '{message.Id}' has been saved successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while saving message to the database.");
            throw;
        }
    }
}