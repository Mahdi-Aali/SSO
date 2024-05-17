using Domain.AggregationRoot.RoleAggregate;
using Domain.AggregationRoot.UserAggregate;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Quartz;

namespace Web.StartupConfiguration.ServiceConfigurationLoadersExtenssion;

public static class WebApplicationServiceConfigurationLoaderExtenssions
{
    public static IServiceCollection LoadServiceConfigurations(this IServiceCollection services, in IConfiguration configuration)
    {
        AddRazorPages(services);
        AddIdentity(services);
        ConfigureApplicationCookies(services);
        AddQuartzScheduler(services);

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
}
