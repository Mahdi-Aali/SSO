using Domain.AggregationRoot.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Application.CQRS.User.Queries;

public class GetUserByUsernameQuery : IRequest<SSOUser>
{
    public required string Username { get; set; } = string.Empty;

    [SetsRequiredMembers]
    public GetUserByUsernameQuery(string username) => (this.Username) = (username);
}

public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, SSOUser>
{
    private readonly UserManager<SSOUser> _userManager;
    private readonly ILogger<GetUserByUsernameQueryHandler> _logger;

    public GetUserByUsernameQueryHandler(UserManager<SSOUser> userManager, ILogger<GetUserByUsernameQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public async Task<SSOUser> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        try
        {
            SSOUser? user = await _userManager.FindByNameAsync(request.Username);
            return user!;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, message: "something went wrong when fetching user by its username from database.");
            return null!;
        }
    }
}
