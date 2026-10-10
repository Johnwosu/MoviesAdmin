using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Data;
using MoviesAdmin.Models;

namespace MoviesAdmin.Controllers;

public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;

    // ASP.NET Core supplies the database context.
    public MoviesController(MoviesAdminContext context)
    {
        _context = context;
    }

    // ==================== INDEX ====================

    // Show all movies, newest release first.
    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies
            .OrderByDescending(movie => movie.TheaterReleaseDate)
            .ToListAsync();

        return View(movies);
    }

    // ==================== CREATE ====================

    // GET: Display an empty form for adding a movie.
    public IActionResult Create()
    {
        return View();
    }

    // POST: Receive, validate and save the new movie.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        Movie movie,
        int runtimeHours,
        int runtimeMinutesPart)
    {
        // Convert hours and minutes into total minutes.
        movie.RuntimeMinutes =
            (runtimeHours * 60) + runtimeMinutesPart;

        // RuntimeMinutes is calculated rather than entered directly.
        ModelState.Remove(nameof(Movie.RuntimeMinutes));

        if (runtimeHours < 0)
        {
            ModelState.AddModelError(
                "runtimeHours",
                "Runtime hours cannot be negative.");
        }

        if (runtimeMinutesPart < 0 || runtimeMinutesPart > 59)
        {
            ModelState.AddModelError(
                "runtimeMinutesPart",
                "Runtime minutes must be between 0 and 59.");
        }

        if (movie.RuntimeMinutes <= 0)
        {
            ModelState.AddModelError(
                "runtimeHours",
                "Movie runtime must be greater than zero.");
        }

        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        movie.Id = 0;

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ==================== DETAILS ====================

    // GET: Display one movie.
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // ==================== EDIT ====================

    // GET: Display the Edit form with existing values.
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: Receive, validate and save changes.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Movie movie,
        int runtimeHours,
        int runtimeMinutesPart)
    {
        if (id != movie.Id)
        {
            return BadRequest();
        }

        // Convert hours and minutes into total minutes.
        movie.RuntimeMinutes =
            (runtimeHours * 60) + runtimeMinutesPart;

        ModelState.Remove(nameof(Movie.RuntimeMinutes));

        if (runtimeHours < 0)
        {
            ModelState.AddModelError(
                "runtimeHours",
                "Runtime hours cannot be negative.");
        }

        if (runtimeMinutesPart < 0 || runtimeMinutesPart > 59)
        {
            ModelState.AddModelError(
                "runtimeMinutesPart",
                "Runtime minutes must be between 0 and 59.");
        }

        if (movie.RuntimeMinutes <= 0)
        {
            ModelState.AddModelError(
                "runtimeHours",
                "Movie runtime must be greater than zero.");
        }

        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        var existingMovie =
            await _context.Movies.FindAsync(id);

        if (existingMovie == null)
        {
            return NotFound();
        }

        // Update only the fields still used in the simplified form.
        existingMovie.Title = movie.Title;
        existingMovie.Synopsis = movie.Synopsis;
        existingMovie.Genre = movie.Genre;
        existingMovie.Rating = movie.Rating;
        existingMovie.RuntimeMinutes = movie.RuntimeMinutes;
        existingMovie.TheaterReleaseDate =
            movie.TheaterReleaseDate;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ==================== DELETE ====================

    // GET: Display deletion confirmation.
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: Delete the selected movie.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}