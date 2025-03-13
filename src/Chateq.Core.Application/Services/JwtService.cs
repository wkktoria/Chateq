using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Chateq.Core.Domain.DTOs;
using Chateq.Core.Domain.Interfaces.Services;
using Chateq.Core.Domain.Models;
using Chateq.Core.Domain.Options;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Chateq.Core.Application.Services;

public class JwtService : IJwtService
{
    private readonly JwtSettingsOption _jwtSettingsOption;

    public JwtService(IOptions<JwtSettingsOption> jwtSettingsOption)
    {
        _jwtSettingsOption = jwtSettingsOption.Value;
    }

    public AuthDto GenerateJwtToken(User user)
    {
        var claims = GetClaims(user);
        var expiryDate = DateTime.Now.AddMinutes(Convert.ToDouble(_jwtSettingsOption.ExpiryInMinutes));
        var credentials = GetCredentials();

        var token = new JwtSecurityToken(
            claims: claims,
            expires: expiryDate,
            signingCredentials: credentials
        );

        return new AuthDto
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            ExpiryDate = expiryDate
        };
    }

    private static Claim[] GetClaims(User user)
    {
        return
        [
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
        ];
    }

    private SigningCredentials GetCredentials()
    {
        var byteSecretKey = Encoding.ASCII.GetBytes(_jwtSettingsOption.SecretKey);
        var key = new SymmetricSecurityKey(byteSecretKey);
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        return credentials;
    }
}