using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Models;

namespace Chateq.Core.Domain.Interfaces.Services;

public interface IJwtService
{
    AuthDto GenerateJwtToken(User user);
}