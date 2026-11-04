using Auth0.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MoviesAdmin.Controllers;

public class AccountController : Controller
{
    // Anyone must be able to reach Login.
    [AllowAnonymous]
    public async Task Login(string returnUrl = "/")
    {
        var authenticationProperties =
            new LoginAuthenticationPropertiesBuilder()
                .WithRedirectUri(returnUrl)
                .Build();

        await HttpContext.ChallengeAsync(
            Auth0Constants.AuthenticationScheme,
            authenticationProperties);
    }

    // Only an authenticated user can log out.
    [Authorize]
    public async Task Logout()
    {
        var authenticationProperties =
            new LogoutAuthenticationPropertiesBuilder()
                .WithRedirectUri(
                    Url.Action("Index", "Home") ?? "/")
                .Build();

        await HttpContext.SignOutAsync(
            Auth0Constants.AuthenticationScheme,
            authenticationProperties);

        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);
    }

    // Only an authenticated user can view their profile.
    [Authorize]
    public IActionResult Profile()
    {
        return View();
    }
}