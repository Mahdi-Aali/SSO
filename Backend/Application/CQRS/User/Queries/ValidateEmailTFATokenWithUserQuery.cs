using Domain.AggregationRoot.UserAggregate;
using Domain.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Application.CQRS.User.Queries;

public class ValidateEmailTFATokenWithUserQuery : IRequest<SysResult<bool>>
{
    public required SSOUser User { get; set; }
    public required string TFAToken { get; set; }

    [SetsRequiredMembers]
    public ValidateEmailTFATokenWithUserQuery(SSOUser user, string tfaToken) => (User, TFAToken) = (user, tfaToken);
}

public class ValidateEmailTFATokenWithUserQueryHandler : IRequestHandler<ValidateEmailTFATokenWithUserQuery, SysResult<bool>>
{
    private readonly UserManager<SSOUser> _userManager;
    private readonly ILogger<ValidateEmailTFATokenWithUserQueryHandler> _logger;
    private readonly IDistributedCache _redisCache;

    public ValidateEmailTFATokenWithUserQueryHandler(
        UserManager<SSOUser> userManager,
        ILogger<ValidateEmailTFATokenWithUserQueryHandler> logger,
        IDistributedCache redisCache) =>
        (_userManager, _logger, _redisCache) = (userManager, logger, redisCache);


    public async Task<SysResult<bool>> Handle(ValidateEmailTFATokenWithUserQuery request, CancellationToken cancellationToken)
    {
        if (await _redisCache.GetStringAsync(request.User.Id.ToString(), cancellationToken) is not null)
        {
            if (await _userManager.VerifyTwoFactorTokenAsync(request.User, "Email", request.TFAToken))
            {
                return new(true);
            }
            return new(false, false, ["Token isn't valid."]);
        }
        return new(false, false, ["Token expired."]);
    }
}