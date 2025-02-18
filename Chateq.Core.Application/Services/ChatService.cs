using System.Text.Json;
using Chateq.Core.Domain.Constants;
using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Producers;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Interfaces.Services;
using Chateq.Core.Domain.Models;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace Chateq.Core.Application.Services;

public class ChatService(IChatRepository chatRepository, IKafkaProducer kafkaProducer, ILogger<ChatService> logger)
    : IChatService
{
    public async Task<ChatDto> GetPaginatedChatAsync(string chatName, int pageNumber, int pageSize)
    {
        var chat = await chatRepository.GetChatWithMessagesAsync(chatName, pageNumber, pageSize);
        var chatDto = ConvertToChatDto(chat);

        return chatDto;
    }

    public async Task SaveMessageAsync(MessageDto messageDto)
    {
        await kafkaProducer.ProduceAsync(TopicKafka.Message, new Message<string, string>
        {
            Key = messageDto.Id.ToString(),
            Value = JsonSerializer.Serialize(messageDto)
        });
    }

    private static ChatDto ConvertToChatDto(Chat chat)
    {
        var chatDto = new ChatDto
        {
            Id = chat.Id,
            Name = chat.Name,
            Messages = chat.Messages
                .OrderByDescending(m => m.CreatedAt)
                .Select(m => new MessageDto
                {
                    Id = m.Id,
                    Sender = m.Sender.Username,
                    SenderId = m.SenderId,
                    MessageText = m.MessageText,
                    CreatedAt = m.CreatedAt,
                }).ToHashSet()
        };

        return chatDto;
    }
}