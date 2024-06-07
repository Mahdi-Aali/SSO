using Domain.AggregationRoot.RoleAggregate;
using Domain.AggregationRoot.UserAggregate;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Quartz;
using System.Reflection;
using StackExchange.Redis;

namespace Web.StartupConfiguration.ServiceConfigurationLoadersExtenssion;

public static class WebApplicationServiceConfigurationLoaderExtenssions
{
    public static IServiceCollection LoadServiceConfigurations(this IServiceCollection services, in IConfiguration configuration, in Assembly[] assemblies)
    {
        AddRazorPages(services);
        AddIdentity(services);
        ConfigureApplicationCookies(services);
        AddQuartzScheduler(services);
        AddMediator(services, assemblies);
        AddRedisCache(services, configuration);

        return services;
    }

    private static void AddRazorPages(IServiceCollection services)
    {
        services.AddRazorPages();
    }

    private static void AddIdentity(IServiceCollection services)
    {
        services.AddIdentity<SSOUser, SSORole>(cfg =>
        {
            cfg.SignIn.RequireConfirmedEmail = true;
            cfg.User.RequireUniqueEmail = true;
            cfg.Lockout.AllowedForNewUsers = true;
            cfg.Lockout.MaxFailedAccessAttempts = 5;
            cfg.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
        })
            .AddEntityFrameworkStores<SSODatabaseContext>()
            .AddDefaultTokenProviders();
    }


    private static void ConfigureApplicationCookies(IServiceCollection services)
    {
        services.ConfigureApplicationCookie(cfg =>
        {
            cfg.LogoutPath = "/Logout";
            cfg.LoginPath = "/Login";
            cfg.AccessDeniedPath = "/Login";
            cfg.ExpireTimeSpan = TimeSpan.FromDays(20);
        });
    }

    private static void AddQuartzScheduler(IServiceCollection services)
    {
        services.AddQuartz();
        services.AddQuartzHostedService(cfg =>
        {
            cfg.WaitForJobsToComplete = true;
        });
    }

    private static void AddMediator(IServiceCollection services, Assembly[] assemblies)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(assemblies);
        });
    }

    private static void AddRedisCache(IServiceCollection services, IConfiguration configuration)
    {
        services.AddStackExchangeRedisCache(cfg =>
        {
            cfg.Configuration = configuration.GetConnectionString("redis-cache");
            cfg.InstanceName = "sso-redis-cache";
        });
    }
}
