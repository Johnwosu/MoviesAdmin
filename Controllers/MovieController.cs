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

    // INDEX: Show all movies, newest release first.
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
    public async Task<IActionResult> Create(Movie movie)
    {
        // Redisplay the form with validation errors and submitted values.
        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        // Reset the ID so SQL Server generates it for the new record.
        movie.Id = 0;

        // Add the movie and save it asynchronously.
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        // Return to the movie list after saving.
        return RedirectToAction(nameof(Index));
    }

    // ==================== DETAILS ====================

    // GET: Find a movie by its ID and display its details.
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        // Return HTTP 404 if the movie does not exist.
        if (movie == null)
        {
            return NotFound();
        }

        // Pass the movie to the Details view.
        return View(movie);
    }

    // ==================== EDIT ====================

    // GET: Display the Edit form with the movie's current values.
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: Receive, validate and save changes to the movie.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Movie movie)
    {
        // Ensure the URL and submitted form identify the same movie.
        if (id != movie.Id)
        {
            return BadRequest();
        }

        // Redisplay the form if any submitted values are invalid.
        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        // Retrieve the existing movie from the database.
        var existingMovie = await _context.Movies.FindAsync(id);

        if (existingMovie == null)
        {
            return NotFound();
        }

        // Copy the submitted values into the existing movie.
        _context.Entry(existingMovie).CurrentValues.SetValues(movie);

        // Save the changes asynchronously.
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // ==================== DELETE ====================

    // GET: Display a confirmation page before deleting the movie.
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: Process the confirmed deletion.
    // ActionName lets this method handle requests to the Delete action.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        // Mark the movie for deletion and save the change.
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}