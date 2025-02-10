using Chateq.Core.Domain;
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
}