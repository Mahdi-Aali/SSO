using Application.CQRS.User.Commands;
using Application.CQRS.User.Queries;
using Application.DTOs.Authentication;
using Domain.AggregationRoot.UserAggregate;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Web.Pages.Login;

[AllowAnonymous]
[ValidateAntiForgeryToken]
public class IndexModel : PageModel
{
    private readonly SignInManager<SSOUser> _signInManager;
    private readonly ILogger<IndexModel> _logger;
    private readonly IMediator _mediator;


    public IndexModel(SignInManager<SSOUser> signInManager, ILogger<IndexModel> logger, IMediator mediator)
    {
        _signInManager = signInManager;
        _logger = logger;
        _mediator = mediator;
    }


    [BindProperty]
    [FromForm]
    public LoginDto LoginDto { get; set; } = null!;


    public void OnGet()
    {
    }


    public async Task<IActionResult> OnPostAsync()
    {
        if (ModelState.IsValid)
        {
            SSOUser? user = await _mediator.Send(new GetUserByUsernameQuery(LoginDto.Username));

            if (user != null)
            {
                var signInResult = await _signInManager.PasswordSignInAsync(user, LoginDto.Password, LoginDto.RememberMe, true);
                if (signInResult.IsLockedOut)
                {
                    ModelState.AddModelError("All", "Your account has been locked. pls try again after 30 minutes.");
                    return Page();
                }

                if (!signInResult.RequiresTwoFactor)
                {
                    if (signInResult.Succeeded)
                    {
                        string? redirectUrl = Request.Form["ReturnUrl"];
                        return Redirect(redirectUrl is not null ? redirectUrl : "/");
                    }
                    ModelState.AddModelError("All", "Wrong password! try again...");
                }
                else
                {
                    if (await _mediator.Send(new GenerateAndSendTwoFactorAuthenticationCodeCommand(user)))
                    {
                        TempData["UserId"] = user.Id;
                        TempData["RememberMe"] = LoginDto.RememberMe;

                        return RedirectToPage("/Login/TFA");
                    }
                    else
                    {
                        ModelState.AddModelError("All", "faild to send TFA code. please try again...");
                    }
                }
            }
            else
            {
                ModelState.AddModelError("All", "There is no account with given username in our website.");
            }
        }

        return Page();
    }
}
