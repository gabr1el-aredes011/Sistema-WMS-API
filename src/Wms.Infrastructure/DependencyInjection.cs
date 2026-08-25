using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Wms.Infrastructure.Persistence;

namespace Wms.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string connectionString)
    {
        services.AddDbContext<WmsDbContext>(options =>
        {
            options.UseNpgsql(connectionString);
        });

        services
            .AddHealthChecks()
            .AddDbContextCheck<WmsDbContext>(
                name: "postgresql",
                tags: new[] { "ready" });

        return services;
    }
}