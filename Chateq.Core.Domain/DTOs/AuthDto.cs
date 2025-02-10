namespace Chateq.Core.Domain.DTOs;

public class AuthDto
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiryDate { get; set; }
}