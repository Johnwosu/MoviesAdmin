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

    // Show all movies, newest release first.
    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies
            .OrderByDescending(movie => movie.TheaterReleaseDate)
            .ToListAsync();

        return View(movies);
    }

    // Show the empty Create form.
    public IActionResult Create()
    {
        return View();
    }

    // Validate and save a new movie.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Movie movie)
    {
        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        // SQL Server generates the new movie's ID.
        movie.Id = 0;

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Find a movie by its ID and show its details.
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        // Return 404 if the movie does not exist.
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // Fill the Edit form with the movie's current values.
    public async Task<IActionResult> Edit(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // Validate and save the submitted changes.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Movie movie)
    {
        // Check that the URL and form identify the same movie.
        if (id != movie.Id)
        {
            return BadRequest();
        }

        // Keep the submitted values when validation fails.
        if (!ModelState.IsValid)
        {
            return View(movie);
        }

        var existingMovie = await _context.Movies.FindAsync(id);

        if (existingMovie == null)
        {
            return NotFound();
        }

        // Copy the form values into the existing database record.
        _context.Entry(existingMovie).CurrentValues.SetValues(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // Show a confirmation page before deleting.
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // Handle the Delete form submission.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie = await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        // Remove the record and save the deletion.
        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}