using Core.DependencyInjection;
using Domain.AggregationRoot.UserAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Infrastructure;

public sealed class InfrastructureDependencyProvider : IDependencyInjectionProvider
{
    public static IServiceCollection GetAssemblyDependencies(IServiceCollection services, IConfiguration configuration)
    {
        AddDbContext(services, configuration);
        AddIdentityCore(services);

        return services;
    }


    public static void AddDbContext(IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SSODatabaseContext>(opt =>
        {
            opt.UseSqlServer(configuration.GetConnectionString("Default"), sqlServerOptions =>
            {
                sqlServerOptions
                .CommandTimeout(TimeSpan.FromSeconds(3).Seconds)
                .EnableRetryOnFailure(5, TimeSpan.FromSeconds(5), null)
                .MigrationsAssembly(typeof(InfrastructureDependencyProvider).Assembly.GetName().Name);
            });
        });
    }



    public static void AddIdentityCore(IServiceCollection services)
    {
        services.AddIdentityCore<SSOUser>(cfg =>
        {
            cfg.User.RequireUniqueEmail = true;
            cfg.SignIn.RequireConfirmedEmail = true;
            cfg.Stores.ProtectPersonalData = true;
        })
            .AddEntityFrameworkStores<SSODatabaseContext>();
    }
}
