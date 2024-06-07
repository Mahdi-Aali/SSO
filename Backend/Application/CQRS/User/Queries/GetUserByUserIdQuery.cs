using Domain.AggregationRoot.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Application.CQRS.User.Queries;

public class GetUserByUserIdQuery : IRequest<SSOUser?>
{
    public required Guid UserId { get; set; }

    [SetsRequiredMembers]
    public GetUserByUserIdQuery(Guid userId)
    {
        this.UserId = userId;
    }
}


public class GetUserByUserIdQueryHandler : IRequestHandler<GetUserByUserIdQuery, SSOUser?>
{
    private UserManager<SSOUser> _userManager;
    private ILogger<GetUserByUserIdQueryHandler> _logger;

    public GetUserByUserIdQueryHandler(UserManager<SSOUser> userManager, ILogger<GetUserByUserIdQueryHandler> logger)
    {
        _userManager = userManager;
        _logger = logger;
    }

    public Task<SSOUser?> Handle(GetUserByUserIdQuery request, CancellationToken cancellationToken) =>
        _userManager.FindByIdAsync(request.UserId.ToString());
}

