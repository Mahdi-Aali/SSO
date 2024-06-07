using Domain.SeedWork;
using Microsoft.AspNetCore.Identity;

namespace Domain.AggregationRoot.RoleAggregate;

public class SSORole : IdentityRole<Guid>, IAggregateRoot
{

}
