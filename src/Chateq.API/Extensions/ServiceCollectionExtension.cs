using System.Text;
using Chateq.Core.Application.Services;
using Chateq.Core.Domain;
using Chateq.Core.Domain.Interfaces.Producers;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Core.Domain.Interfaces.Services;
using Chateq.Core.Domain.Options;
using Chateq.Infrastructure.Producers;
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

        services.AddTransient<IJwtService, JwtService>();

        services.AddTransient<IChatRepository, ChatRepository>();
        services.AddTransient<IChatService, ChatService>();
        
        services.AddTransient<IKafkaProducer, KafkaProducer>();
        
        services.AddSingleton<IUserConnectionService, UserConnectionService>();

        services.AddSignalR();

        return services;
    }

    public static IServiceCollection AddOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JwtSettingsOption>(options =>
            configuration.GetSection(nameof(JwtSettingsOption)).Bind(options));
        services.Configure<KafkaOption>(options =>
            configuration.GetSection(nameof(KafkaOption)).Bind(options));
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
            options.Events = GetEvents();
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

    private static JwtBearerEvents GetEvents()
    {
        return new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["token"];
                var path = context.HttpContext.Request.Path;

                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/messageHub"))
                {
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            }
        };
    }
}