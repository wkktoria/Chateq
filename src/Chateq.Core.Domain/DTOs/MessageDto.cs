namespace Chateq.Core.Domain.DTOs;

public class MessageDto
{
    public Guid Id { get; set; } = Guid.NewGuid();

    public string Sender { get; set; } = string.Empty;

    public string MessageText { get; set; } = string.Empty;

    public Guid ChatId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public Guid SenderId { get; set; }
}