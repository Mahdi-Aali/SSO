using Domain.AggregationRoot.RoleAggregate;
using Domain.AggregationRoot.UserAggregate;
using Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Quartz;

namespace Web.Modules.QuartzJobs;

public sealed class DatabaseSeedDataJob : IJob
{
    private readonly SSODatabaseContext _dbContext;
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<DatabaseSeedDataJob> _logger;

    public DatabaseSeedDataJob(SSODatabaseContext dbContext, IServiceProvider serviceProvider, ILogger<DatabaseSeedDataJob> logger) =>
        (_dbContext, _serviceProvider, _logger) = (dbContext, serviceProvider, logger);

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            await SeedData(_serviceProvider, context.CancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong when seeding the data base.");
        }
        finally
        {
            await Task.CompletedTask;
        }
    }


    private void LogInformation(string message)
    {
        _logger.LogInformation(message);
    }

    private async Task<bool> IsPendingMigrationsExist(CancellationToken cancellationToken = default) => (await GetPendingMigrationsAsync(cancellationToken)).Any();

    private async Task<int> GetPendingMigrationsCount(CancellationToken cancellationToken = default) => (await GetPendingMigrationsAsync(cancellationToken)).Count();

    private async Task<IEnumerable<string>> GetPendingMigrationsAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Database.GetPendingMigrationsAsync(cancellationToken);

    private async Task CreateDatabase(CancellationToken cancellationToken = default)
    {
        LogInformation("Checking that if database is initialized or not.");

        if (await IsPendingMigrationsExist(cancellationToken))
        {
            int pendingMigrationsCount = await GetPendingMigrationsCount(cancellationToken);

            LogInformation($"{pendingMigrationsCount} pending migration(s) found. Start migrating pending migrations.");

            await _dbContext.Database.MigrateAsync(cancellationToken);

            LogInformation("Successfully seed database...");
        }
        else
        {
            LogInformation("No pending migrations found.");
        }
    }

    private async Task AddDefaultRoles(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        RoleManager<SSORole> roleManager = serviceProvider.GetRequiredService<RoleManager<SSORole>>();
        LogInformation("Checking if that system default roles exist...");

        if (!await roleManager.RoleExistsAsync("SSOAdmin"))
        {
            string[] roles = ["SSOAdmin", "SSOUser"];
            foreach(string role in roles)
            {
                LogInformation($"Adding {role} to system.");
                IdentityResult roleAdditionResult = await roleManager.CreateAsync(new() { Name = role });
                if (roleAdditionResult.Succeeded)
                {
                    LogInformation($"Adding {role} to system was successfull.");
                }
                else
                {
                    _logger.LogWarning($"Faild to add {role} to system.", roleAdditionResult.Errors.Select(s => s.Description).ToArray());
                }
            }
        }
        else
        {
            LogInformation("System default roles are already exists.");
        }

    }

    private async Task<SSOUser> CreateAdminAccount(UserManager<SSOUser> userManager, CancellationToken cancellationToken = default)
    {
        SSOUser admin = new SSOUser()
        {
            UserName = "Admin",
            AccessFailedCount = 5,
            Email = "Admin@MahdiAaliSSO.com",
            EmailConfirmed = true,
            PhoneNumber = "+989058785110",
            PhoneNumberConfirmed = true,
            TwoFactorEnabled = true,
            LockoutEnabled = true
        };

        IdentityResult createAccountResult = await userManager.CreateAsync(admin, "Admin@123");

        if (createAccountResult.Succeeded)
        {
            LogInformation("Admin account created successfully.");
            return admin;
        }
        else
        {
            _logger.LogWarning("Admin account were not created.", createAccountResult.Errors.Select(s => s.Description).ToArray());
            return null!;
        }
    }

    private async Task AddSystemAdminRoleToAdminAccount(UserManager<SSOUser> userManager, SSOUser admin, CancellationToken cancellationToken = default)
    {
        LogInformation("Adding SSOAdmin role to admin account.");
        IdentityResult roleAdditionResult = await userManager.AddToRoleAsync(admin, "SSOAdmin");
        if (roleAdditionResult.Succeeded)
        {
            LogInformation("SSOAdmin role successfully added to admin account.");
        }
        else
        {
            _logger.LogWarning("Faild to add SSOAdmin role to admin account.", roleAdditionResult.Errors.Select(s => s.Description).ToArray());
        }
    }

    private async Task SeedAdminAccount(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        UserManager<SSOUser> userManager = services.GetRequiredService<UserManager<SSOUser>>();
        SSOUser? admin = await _dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.NormalizedUserName == "ADMIN", cancellationToken);
        if (admin is null)
        {
            admin = await CreateAdminAccount(userManager, cancellationToken);
        }
        else
        {
            if (!await userManager.IsInRoleAsync(admin, "SSOAdmin"))
            {
                await AddSystemAdminRoleToAdminAccount(userManager, admin, cancellationToken);
            }
        }
    }

    private async Task SeedData(IServiceProvider serviceProvider, CancellationToken cancellationToken = default)
    {
        await CreateDatabase(cancellationToken);

        IServiceProvider sp = serviceProvider.CreateAsyncScope().ServiceProvider;

        await AddDefaultRoles(sp, cancellationToken);

        await SeedAdminAccount(sp, cancellationToken);
    }
}
