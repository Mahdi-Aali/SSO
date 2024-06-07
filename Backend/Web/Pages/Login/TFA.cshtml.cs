using Application.CQRS.User.Queries;
using Application.DTOs.Authentication;
using Domain.AggregationRoot.UserAggregate;
using Domain.SeedWork;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Login;

[ValidateAntiForgeryToken]
[AllowAnonymous]
public class TFAModel : PageModel
{
    private readonly IMediator _mediator;
    private readonly ILogger<TFAModel> _logger;
    private readonly SignInManager<SSOUser> _signInManager;

    public TFAModel(IMediator mediator, ILogger<TFAModel> logger, SignInManager<SSOUser> signInManager)
    {
        _mediator = mediator;
        _logger = logger;
        _signInManager = signInManager;
    }


    [BindProperty]
    [FromForm]
    public TFADto TFADto { get; set; } = null!;


    public IActionResult OnGet()
    {
        if (TempData["UserId"] is not null && TempData["UserId"] is Guid userId)
        {
            bool? rememberMe = (bool)TempData["RememberMe"]!;
            TFADto = new()
            {
                UserId = userId,
                RememberMe = rememberMe ?? false
            };
            return Page();
        }
        return Redirect("/");
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            SSOUser? user = await _mediator.Send(new GetUserByUserIdQuery(TFADto.UserId));
            if (User is not null)
            {
                SysResult<bool> tfaTokenVerificationResult = await _mediator.Send(new ValidateEmailTFATokenWithUserQuery(user!, TFADto.TFAToken));
                if (tfaTokenVerificationResult.IsSuccessfull)
                {
                    var signInResult = await _signInManager.TwoFactorSignInAsync("Email", TFADto.TFAToken, false, true);
                    if (signInResult.Succeeded)
                    {
                        return Redirect("/");
                    }
                    else if (signInResult.IsLockedOut)
                    {
                        ModelState.AddModelError("All", "Your account has been locked. pls try again after 30 minutes.");
                    }
                    else
                    {
                        ModelState.AddModelError("All", "Faild to signin with two factor authentication token.");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("All", "Invalid user.");
            }
        }
        return Page();
    }
}
