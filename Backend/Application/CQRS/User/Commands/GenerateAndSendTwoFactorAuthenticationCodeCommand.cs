using Domain.AggregationRoot.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using System.Diagnostics.CodeAnalysis;

namespace Application.CQRS.User.Commands;

public class GenerateAndSendTwoFactorAuthenticationCodeCommand : IRequest<bool>
{
    public required SSOUser User { get; set; } = null!;


    [SetsRequiredMembers]
    public GenerateAndSendTwoFactorAuthenticationCodeCommand(SSOUser user)
    {
        User = user;
    }
}


public class GenerateAndSendTwoFactorAuthenticationCodeCommandHandler : IRequestHandler<GenerateAndSendTwoFactorAuthenticationCodeCommand, bool>
{
    private readonly UserManager<SSOUser> _userManager;
    private readonly ILogger<GenerateAndSendTwoFactorAuthenticationCodeCommandHandler> _logger;
    private readonly IDistributedCache _redisCache;

    public GenerateAndSendTwoFactorAuthenticationCodeCommandHandler(UserManager<SSOUser> userManager,
        ILogger<GenerateAndSendTwoFactorAuthenticationCodeCommandHandler> logger,
        IDistributedCache redisCache)
    {
        _userManager = userManager;
        _logger = logger;
        _redisCache = redisCache;
    }

    public async Task<bool> Handle(GenerateAndSendTwoFactorAuthenticationCodeCommand request, CancellationToken cancellationToken)
    {
        try
        {
            string token = await _userManager.GenerateTwoFactorTokenAsync(request.User, "Email");

            _logger.LogInformation($"TAF token: {token}");

            _redisCache.SetString(request.User.Id.ToString(), token, new DistributedCacheEntryOptions()
            {
                SlidingExpiration = TimeSpan.FromSeconds(90)
            });

            // call email sending microservice here...

            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Something went wrong when generating and sending two factor authentication code.");
            return false;
        }
    }
}

