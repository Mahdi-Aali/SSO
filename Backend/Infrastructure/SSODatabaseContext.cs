using Domain.AggregationRoot.RoleAggregate;
using Domain.AggregationRoot.UserAggregate;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;

public partial class SSODatabaseContext : IdentityDbContext<SSOUser, SSORole, Guid>
{
    public SSODatabaseContext(DbContextOptions<SSODatabaseContext> opt) : base(opt)
    {

    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.ApplyConfigurationsFromAssembly(GetType().Assembly);
    }
}
