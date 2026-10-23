using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Data;
using MoviesAdmin.Models;
using MoviesAdmin.Services;

namespace MoviesAdmin.Controllers;

public class MoviesController : Controller
{
    private readonly MoviesAdminContext _context;
    private readonly IAzureBlobService _blobService;

    // ASP.NET Core supplies the database context
    // and Azure Blob Storage service.
    public MoviesController(
        MoviesAdminContext context,
        IAzureBlobService blobService)
    {
        _context = context;
        _blobService = blobService;
    }

    // ==================== INDEX ====================

    // Display all movies, newest release first.
    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies
            .OrderByDescending(movie => movie.TheaterReleaseDate)
            .ToListAsync();

        return View(movies);
    }

    // ==================== CREATE ====================

// GET: Display the form for creating a new movie.
public IActionResult Create()
{
    return View();
}

// POST: Receive the Create form,
// validate the movie information,
// upload the poster image,
// and save the movie to the database.
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create(
    [Bind("Id,Title,Synopsis,Genre,Rating,TheaterReleaseDate")]
    Movie movie,
    int? runtimeHours,
    int? runtimeMinutesPart,
    IFormFile? imageFile)
{
    // RuntimeMinutes is calculated by this controller
    // instead of being entered directly on the form.
    ModelState.Remove(nameof(Movie.RuntimeMinutes));

    // ==================== RUNTIME VALIDATION ====================

    // Runtime Hours must be entered.
    if (runtimeHours == null)
    {
        ModelState.AddModelError(
            "runtimeHours",
            "Runtime hours are required.");
    }
    else if (runtimeHours < 0)
    {
        ModelState.AddModelError(
            "runtimeHours",
            "Runtime hours cannot be negative.");
    }

    // Runtime Minutes must be entered.
    if (runtimeMinutesPart == null)
    {
        ModelState.AddModelError(
            "runtimeMinutesPart",
            "Runtime minutes are required.");
    }
    else if (runtimeMinutesPart < 0 ||
             runtimeMinutesPart > 59)
    {
        ModelState.AddModelError(
            "runtimeMinutesPart",
            "Runtime minutes must be between 0 and 59.");
    }

    // Convert hours and minutes into total runtime minutes.
    movie.RuntimeMinutes =
        ((runtimeHours ?? 0) * 60) +
        (runtimeMinutesPart ?? 0);

    // If both runtime fields were entered,
    // the total runtime must be greater than zero.
    if (runtimeHours != null &&
        runtimeMinutesPart != null &&
        movie.RuntimeMinutes <= 0)
    {
        ModelState.AddModelError(
            "runtimeHours",
            "Movie runtime must be greater than zero.");
    }

    // ==================== IMAGE VALIDATION ====================

    // A poster image is required when creating a new movie.
    if (imageFile == null ||
        imageFile.Length == 0)
    {
        ModelState.AddModelError(
            "imageFile",
            "The movie image field is required.");
    }

    // ==================== CHECK VALIDATION ====================

    // If any validation failed,
    // return to the Create page and display the errors.
    if (!ModelState.IsValid)
    {
        return View(movie);
    }

    // ==================== IMAGE UPLOAD ====================

    // The image is guaranteed to exist here
    // because it passed validation above.
    if (imageFile != null &&
        imageFile.Length > 0)
    {
        // Get the original file extension,
        // for example .jpg or .png.
        string extension =
            Path.GetExtension(imageFile.FileName);

        // Generate a unique filename using a GUID.
        string fileName =
            $"{Guid.NewGuid()}{extension}";

        // Upload the image to Blob Storage
        // and receive the full Blob URL.
        string imageUrl =
            await _blobService.UploadFileAsync(
                imageFile,
                fileName);

        // Save the Blob URL with the movie.
        movie.ImageUrl = imageUrl;
    }

    // ==================== SAVE MOVIE ====================

    // SQL Server generates the movie ID.
    movie.Id = 0;

    // Add the movie to the database.
    _context.Movies.Add(movie);

    // Save the database changes.
    await _context.SaveChangesAsync();

    // Return to the movie list.
    return RedirectToAction(nameof(Index));
}

    // ==================== DETAILS ====================

    // Display the details of one movie.
    public async Task<IActionResult> Details(int id)
    {
        // Find the movie using its ID.
        var movie =
            await _context.Movies.FindAsync(id);

        // Return 404 if the movie does not exist.
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // ==================== EDIT ====================

    // GET: Display the Edit form
    // with the movie's current information.
    public async Task<IActionResult> Edit(int id)
    {
        // Find the movie using its ID.
        var movie =
            await _context.Movies.FindAsync(id);

        // Return 404 if the movie does not exist.
        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }

    // POST: Validate and save changes to a movie.
    // The user can optionally upload a replacement poster.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        Movie movie,
        int runtimeHours,
        int runtimeMinutesPart,
        IFormFile? imageFile)
    {
        // Make sure the movie ID in the URL
        // matches the movie submitted from the form.
        if (id != movie.Id)
        {
            return BadRequest();
        }

        // Convert hours and minutes
        // into total runtime minutes.
        movie.RuntimeMinutes =
            (runtimeHours * 60) + runtimeMinutesPart;

        // RuntimeMinutes is calculated manually.
        ModelState.Remove(nameof(Movie.RuntimeMinutes));

        // Validate runtime hours.
        if (runtimeHours < 0)
        {
            ModelState.AddModelError(
                "runtimeHours",
                "Runtime hours cannot be negative.");
        }

        // Validate runtime minutes.
        if (runtimeMinutesPart < 0 ||
            runtimeMinutesPart > 59)
        {
            ModelState.AddModelError(
                "runtimeMinutesPart",
                "Runtime minutes must be between 0 and 59.");
        }

        // Require the movie to have a runtime.
        if (movie.RuntimeMinutes <= 0)
        {
            ModelState.AddModelError(
                "runtimeHours",
                "Movie runtime is required.");
        }

        // Retrieve the existing movie from the database.
        var existingMovie =
            await _context.Movies.FindAsync(id);

        if (existingMovie == null)
        {
            return NotFound();
        }

        // If validation fails, keep the current
        // image visible on the Edit page.
        if (!ModelState.IsValid)
        {
            movie.ImageUrl =
                existingMovie.ImageUrl;

            return View(movie);
        }

        // Update the movie information.
        existingMovie.Title =
            movie.Title;

        existingMovie.Synopsis =
            movie.Synopsis;

        existingMovie.Genre =
            movie.Genre;

        existingMovie.Rating =
            movie.Rating;

        existingMovie.RuntimeMinutes =
            movie.RuntimeMinutes;

        existingMovie.TheaterReleaseDate =
            movie.TheaterReleaseDate;

        // Only replace the poster if
        // the user selected a new image.
        if (imageFile != null &&
            imageFile.Length > 0)
        {
            // Get the extension of the replacement file.
            string extension =
                Path.GetExtension(imageFile.FileName);

            // Generate a new unique Blob filename.
            string fileName =
                $"{Guid.NewGuid()}{extension}";

            // Upload the replacement poster.
            string imageUrl =
                await _blobService.UploadFileAsync(
                    imageFile,
                    fileName);

            // Change the movie record to point
            // to the newly uploaded poster.
            existingMovie.ImageUrl =
                imageUrl;
        }

        // If no new image was selected,
        // existingMovie.ImageUrl is left unchanged.

        // Save all movie changes.
        await _context.SaveChangesAsync();

        // Return to the movie list.
        return RedirectToAction(nameof(Index));
    }

    // ==================== DELETE ====================

    // GET: Display the delete confirmation page.
    // This does NOT delete the movie.
    public async Task<IActionResult> Delete(int id)
    {
        // Find the movie that the user wants to delete.
        var movie =
            await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        // Display the Delete confirmation page.
        return View(movie);
    }

    // POST: Actually delete the movie
    // after the user confirms on the Delete page.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Find the movie in the database.
        var movie =
            await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        // Remove the movie from SQL Server.
        _context.Movies.Remove(movie);

        // Save the deletion.
        await _context.SaveChangesAsync();

        // Return to the movie list.
        return RedirectToAction(nameof(Index));
    }
}