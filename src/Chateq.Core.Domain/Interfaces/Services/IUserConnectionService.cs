using System.Security.Claims;

namespace Chateq.Core.Domain.Interfaces.Services;

public interface IUserConnectionService
{
    void AddConnection(string username, string connectionId);

    void RemoveConnection(string username);

    string GetClaimValue(ClaimsPrincipal? user, string claimName);
}