namespace Chateq.Core.Domain.DTOs;

public class ChatDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public HashSet<MessageDto> Messages { get; set; } = [];
}