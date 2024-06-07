using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureDependencyProvider
{
    public static IServiceCollection LoadInfrastrutureDependencies(this IServiceCollection services, in IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        return services;
    }


    private static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SSODatabaseContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("Default"), sqlServerOptions =>
            {
                sqlServerOptions
                .CommandTimeout(TimeSpan.FromSeconds(3).Seconds)
                .EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null)
                .MigrationsAssembly("Web");
            })
            .EnableDetailedErrors()
            .EnableSensitiveDataLogging();
        });
    }
}
