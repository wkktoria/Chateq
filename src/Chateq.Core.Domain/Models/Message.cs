namespace Chateq.Core.Domain.Models;

public class Message
{
    public Guid Id { get; set; }
    
    public Guid ChatId { get; set; }
    
    public Guid SenderId { get; set; }
    
    public string MessageText { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }

    public Chat Chat { get; set; } = null!;
    
    public User Sender { get; set; } = null!;
}