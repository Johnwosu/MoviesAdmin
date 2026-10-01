using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Data;
using MoviesAdmin.Models;

namespace MoviesAdmin.Controllers;

public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;

    // ASP.NET Core supplies the database context to this controller.
    public MoviesController(MoviesAdminContext context)
    {
        _context = context;
    }

    // GET /Movies: retrieve all movies, newest release first.
    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies
            .OrderByDescending(movie => movie.TheaterReleaseDate)
            .ToListAsync();

        return View(movies);
    }

    // GET /Movies/Create: show the empty Create form.
    public IActionResult Create()
    {
        return View();
    }

    // POST /Movies/Create: receive and save the submitted form.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie)
    {
        // If validation fails, show the form again with its errors.
        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        // Add the movie to the context, then save it to SQL Server.
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        // Return to the movie list after saving.
        return RedirectToAction(nameof(Index));
    }
}