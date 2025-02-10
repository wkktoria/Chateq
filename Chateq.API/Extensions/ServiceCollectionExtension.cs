using System.Text;
using Chateq.Core.Application.Services;
using Chateq.Core.Domain;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Interfaces.Services;
using Chateq.Core.Domain.Options;
using Chateq.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace Chateq.API.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        AddCustomAuthentication(services, configuration);

        var connectionString = configuration.GetValue<string>("ConnectionString");

        services.AddDbContext<ChateqDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        services.AddTransient<IAuthService, AuthService>();

        return services;
    }

    private static void AddCustomAuthentication(IServiceCollection services, IConfiguration configuration)
    {
        var jwtSettings = configuration.GetSection(nameof(JwtSettingsOption)).Get<JwtSettingsOption>();

        if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.SecretKey))
        {
            throw new ArgumentException("The secret key is missing.");
        }

        var key = Encoding.ASCII.GetBytes(jwtSettings.SecretKey);

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.TokenValidationParameters = GetTokenValidationParameters(key);
        });
    }

    private static TokenValidationParameters GetTokenValidationParameters(byte[] key)
    {
        return new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(120)
        };
    }
}