using Chateq.Core.Domain;
using Chateq.Core.Domain.Interfaces.Repositories;
using Chateq.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Chateq.API.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetValue<string>("ConnectionString");

        services.AddDbContext<ChateqDbContext>(options =>
            options.UseSqlServer(connectionString)
        );

        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        return services;
    }
}