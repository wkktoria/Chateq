namespace Chateq.Core.Domain.Models;

public class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = string.Empty;
    
    public string Password { get; set; } = string.Empty;
    
    public DateTime CreatedAt { get; set; }
}