using System.Collections.Concurrent;
using System.Security.Claims;
using Chateq.Core.Domain.Interfaces.Services;

namespace Chateq.Core.Application.Services;

public class UserConnectionService : IUserConnectionService
{
    private readonly ConcurrentDictionary<string, string> _userConnections = new();

    public void AddConnection(string username, string connectionId)
    {
        if (!string.IsNullOrEmpty(username))
        {
            _userConnections[username] = connectionId;
        }
    }

    public void RemoveConnection(string username)
    {
        if (!string.IsNullOrEmpty(username))
        {
            _userConnections.TryRemove(username, out _);
        }
    }

    public string GetClaimValue(ClaimsPrincipal? user, string claimName)
    {
        if (user == null)
        {
            throw new ArgumentException("The user is empty.");
        }

        var claimValue = user.Claims.FirstOrDefault(c => c.Type == claimName)?.Value;

        if (string.IsNullOrEmpty(claimValue))
        {
            throw new ArgumentException($"The claim of type '{claimName}' is empty.");
        }

        return claimValue;
    }
}