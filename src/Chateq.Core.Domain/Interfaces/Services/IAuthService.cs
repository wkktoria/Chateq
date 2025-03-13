using Chateq.Core.Domain.DTOs;

namespace Chateq.Core.Domain.Interfaces.Services;

public interface IAuthService
{
    Task RegisterUserAsync(RegisterUserDto registerUser);

    Task<AuthDto> GetTokenAsync(LoginDto loginModel);
}