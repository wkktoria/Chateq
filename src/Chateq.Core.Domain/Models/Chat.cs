namespace Chateq.Core.Domain.Models;

public class Chat
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }

    public ICollection<Message> Messages { get; set; } = [];
}