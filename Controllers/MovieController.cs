using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
    // Include Genre and AudienceRating so their titles
    // can be displayed in the Index view.
    public async Task<IActionResult> Index()
    {
        var movies = await _context.Movies
            .Include(movie => movie.Genre)
            .Include(movie => movie.AudienceRating)
            .OrderByDescending(movie => movie.TheaterReleaseDate)
            .ToListAsync();

        return View(movies);
    }


    // ==================== CREATE ====================

    // GET: Display the form for creating a new movie.
    public IActionResult Create()
    {
        // Load Genre choices for the dropdown.
        ViewData["GenreId"] =
            new SelectList(
                _context.Genres.OrderBy(genre => genre.Title),
                "Id",
                "Title");

        // Load Audience Rating choices for the dropdown.
        ViewData["AudienceRatingId"] =
            new SelectList(
                _context.AudienceRatings
                    .OrderBy(rating => rating.Title),
                "Id",
                "Title");

        return View();
    }


    // POST: Validate the movie,
    // upload the poster image,
    // and save the movie to the database.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind(
            "Id,Title,Synopsis,GenreId,AudienceRatingId,TheaterReleaseDate")]
        Movie movie,
        int? runtimeHours,
        int? runtimeMinutesPart,
        IFormFile? imageFile)
    {
        // RuntimeMinutes is calculated by this controller
        // instead of being entered directly on the form.
        ModelState.Remove(nameof(Movie.RuntimeMinutes));


        // ==================== GENRE VALIDATION ====================

        // A movie must belong to one Genre.
        if (movie.GenreId <= 0)
        {
            ModelState.AddModelError(
                nameof(Movie.GenreId),
                "Please select a genre.");
        }


        // ==================== AUDIENCE RATING VALIDATION ====================

        // A movie must belong to one Audience Rating.
        if (movie.AudienceRatingId <= 0)
        {
            ModelState.AddModelError(
                nameof(Movie.AudienceRatingId),
                "Please select a content rating.");
        }


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
        // total runtime must be greater than zero.
        if (runtimeHours != null &&
            runtimeMinutesPart != null &&
            movie.RuntimeMinutes <= 0)
        {
            ModelState.AddModelError(
                "runtimeHours",
                "Movie runtime must be greater than zero.");
        }


        // ==================== IMAGE VALIDATION ====================

        // A poster image is required when creating a movie.
        if (imageFile == null ||
            imageFile.Length == 0)
        {
            ModelState.AddModelError(
                "imageFile",
                "The movie image field is required.");
        }


        // ==================== CHECK VALIDATION ====================

        if (!ModelState.IsValid)
        {
            // Reload the Genre dropdown.
            ViewData["GenreId"] =
                new SelectList(
                    _context.Genres.OrderBy(
                        genre => genre.Title),
                    "Id",
                    "Title",
                    movie.GenreId);

            // Reload the Audience Rating dropdown.
            ViewData["AudienceRatingId"] =
                new SelectList(
                    _context.AudienceRatings.OrderBy(
                        rating => rating.Title),
                    "Id",
                    "Title",
                    movie.AudienceRatingId);

            return View(movie);
        }


        // ==================== IMAGE UPLOAD ====================

        if (imageFile != null &&
            imageFile.Length > 0)
        {
            // Keep the original file extension.
            string extension =
                Path.GetExtension(imageFile.FileName);

            // Generate a unique filename using a GUID.
            string fileName =
                $"{Guid.NewGuid()}{extension}";

            // Upload the image to Blob Storage.
            string imageUrl =
                await _blobService.UploadFileAsync(
                    imageFile,
                    fileName);

            // Store the Blob URL with the movie.
            movie.ImageUrl = imageUrl;
        }


        // ==================== SAVE MOVIE ====================

        // SQL Server generates the ID.
        movie.Id = 0;

        _context.Movies.Add(movie);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // ==================== DETAILS ====================

    // Display one movie including its related
    // Genre and Audience Rating.
    public async Task<IActionResult> Details(int id)
    {
        var movie = await _context.Movies
            .Include(movie => movie.Genre)
            .Include(movie => movie.AudienceRating)
            .FirstOrDefaultAsync(
                movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }


    // ==================== EDIT ====================

    // GET: Display the Edit form.
    public async Task<IActionResult> Edit(int id)
    {
        var movie =
            await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        // Load Genre choices and select
        // the movie's current Genre.
        ViewData["GenreId"] =
            new SelectList(
                _context.Genres.OrderBy(
                    genre => genre.Title),
                "Id",
                "Title",
                movie.GenreId);

        // Load Audience Rating choices and select
        // the movie's current Audience Rating.
        ViewData["AudienceRatingId"] =
            new SelectList(
                _context.AudienceRatings.OrderBy(
                    rating => rating.Title),
                "Id",
                "Title",
                movie.AudienceRatingId);

        return View(movie);
    }


    // POST: Validate and save changes.
    // A replacement poster is optional.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind(
            "Id,Title,Synopsis,GenreId,AudienceRatingId,TheaterReleaseDate")]
        Movie movie,
        int? runtimeHours,
        int? runtimeMinutesPart,
        IFormFile? imageFile)
    {
        // Make sure the URL ID matches
        // the submitted movie ID.
        if (id != movie.Id)
        {
            return BadRequest();
        }

        // RuntimeMinutes is calculated manually.
        ModelState.Remove(nameof(Movie.RuntimeMinutes));


        // ==================== GENRE VALIDATION ====================

        if (movie.GenreId <= 0)
        {
            ModelState.AddModelError(
                nameof(Movie.GenreId),
                "Please select a genre.");
        }


        // ==================== AUDIENCE RATING VALIDATION ====================

        if (movie.AudienceRatingId <= 0)
        {
            ModelState.AddModelError(
                nameof(Movie.AudienceRatingId),
                "Please select a content rating.");
        }


        // ==================== RUNTIME VALIDATION ====================

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

        // Convert runtime to total minutes.
        movie.RuntimeMinutes =
            ((runtimeHours ?? 0) * 60) +
            (runtimeMinutesPart ?? 0);

        if (runtimeHours != null &&
            runtimeMinutesPart != null &&
            movie.RuntimeMinutes <= 0)
        {
            ModelState.AddModelError(
                "runtimeHours",
                "Movie runtime must be greater than zero.");
        }


        // Retrieve the existing movie.
        var existingMovie =
            await _context.Movies.FindAsync(id);

        if (existingMovie == null)
        {
            return NotFound();
        }


        // ==================== CHECK VALIDATION ====================

        if (!ModelState.IsValid)
        {
            // Keep the current poster visible.
            movie.ImageUrl =
                existingMovie.ImageUrl;

            // Reload Genre dropdown.
            ViewData["GenreId"] =
                new SelectList(
                    _context.Genres.OrderBy(
                        genre => genre.Title),
                    "Id",
                    "Title",
                    movie.GenreId);

            // Reload Audience Rating dropdown.
            ViewData["AudienceRatingId"] =
                new SelectList(
                    _context.AudienceRatings.OrderBy(
                        rating => rating.Title),
                    "Id",
                    "Title",
                    movie.AudienceRatingId);

            return View(movie);
        }


        // ==================== UPDATE MOVIE ====================

        existingMovie.Title =
            movie.Title;

        existingMovie.Synopsis =
            movie.Synopsis;

        // Store the selected Genre foreign key.
        existingMovie.GenreId =
            movie.GenreId;

        // Store the selected Audience Rating foreign key.
        existingMovie.AudienceRatingId =
            movie.AudienceRatingId;

        existingMovie.RuntimeMinutes =
            movie.RuntimeMinutes;

        existingMovie.TheaterReleaseDate =
            movie.TheaterReleaseDate;


        // ==================== REPLACE IMAGE ====================

        // Image replacement is optional when editing.
        if (imageFile != null &&
            imageFile.Length > 0)
        {
            // Keep the extension of the new image.
            string extension =
                Path.GetExtension(imageFile.FileName);

            // Generate a unique Blob filename.
            string fileName =
                $"{Guid.NewGuid()}{extension}";

            // Upload the replacement image.
            string imageUrl =
                await _blobService.UploadFileAsync(
                    imageFile,
                    fileName);

            // Point the movie to the new image.
            existingMovie.ImageUrl =
                imageUrl;
        }

        // If no new image was selected,
        // the existing ImageUrl remains unchanged.

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // ==================== DELETE ====================

    // GET: Display the Delete confirmation page.
    public async Task<IActionResult> Delete(int id)
    {
        var movie = await _context.Movies
            .Include(movie => movie.Genre)
            .Include(movie => movie.AudienceRating)
            .FirstOrDefaultAsync(
                movie => movie.Id == id);

        if (movie == null)
        {
            return NotFound();
        }

        return View(movie);
    }


    // POST: Delete the movie only after confirmation.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var movie =
            await _context.Movies.FindAsync(id);

        if (movie == null)
        {
            return NotFound();
        }

        _context.Movies.Remove(movie);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}