using Microsoft.AspNetCore.Identity;

namespace Domain.AggregationRoot.UserAggregate;

public sealed class SSOUser : IdentityUser<Guid>
{

}