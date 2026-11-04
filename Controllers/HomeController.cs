using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MoviesAdmin.Models;

namespace MoviesAdmin.Controllers;

public class HomeController : Controller
{
    // Anyone can view the Home page.
    [AllowAnonymous]
    public IActionResult Index()
    {
        return View();
    }

    // Anyone can view the Privacy page.
    [AllowAnonymous]
    public IActionResult Privacy()
    {
        return View();
    }

    // Display the error page.
    [ResponseCache(
        Duration = 0,
        Location = ResponseCacheLocation.None,
        NoStore = true)]
    public IActionResult Error()
    {
        return View(
            new ErrorViewModel
            {
                RequestId =
                    Activity.Current?.Id ??
                    HttpContext.TraceIdentifier
            });
    }
}