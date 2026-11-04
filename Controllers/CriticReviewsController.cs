using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MoviesAdmin;
using MoviesAdmin.Data;
using MoviesAdmin.Models;

namespace MoviesAdmin.Controllers;

[Authorize(Roles = "Critic")]
public class CriticReviewsController : Controller
{
    private readonly MoviesAdminContext _context;

    public CriticReviewsController(
        MoviesAdminContext context)
    {
        _context = context;
    }


    // ==================== INDEX ====================

    // Display only reviews created by the logged-in critic.
    public async Task<IActionResult> Index()
    {
        var userId = User.GetUserId();

        var reviews = await _context.CriticReviews
            .Include(review => review.Movie)
            .Where(review => review.CreatedBy == userId)
            .OrderByDescending(review => review.CreatedDate)
            .ToListAsync();

        return View(reviews);
    }


    // ==================== CREATE ====================

    // GET: Display the form for creating a review.
    public async Task<IActionResult> Create(int movieId)
    {
        var userId = User.GetUserId();

        // Check whether this critic already reviewed the movie.
        var existingReview =
            await _context.CriticReviews
                .FirstOrDefaultAsync(
                    review =>
                        review.MovieId == movieId &&
                        review.CreatedBy == userId);

        if (existingReview != null)
        {
            // A draft can still be edited.
            if (!existingReview.IsPublished)
            {
                return RedirectToAction(
                    nameof(Edit),
                    new { id = existingReview.Id });
            }

            // A published review is locked.
            TempData["ReviewMessage"] =
                "You have already published a review for this movie.";

            return RedirectToAction(nameof(Index));
        }

        var movie = await _context.Movies
            .FirstOrDefaultAsync(
                movie => movie.Id == movieId);

        if (movie == null)
        {
            return NotFound();
        }

        var criticReview = new CriticReview
        {
            MovieId = movie.Id,
            Movie = movie
        };

        return View(criticReview);
    }


    // POST: Save a new critic review.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        [Bind("MovieId,Description,Rating")]
        CriticReview criticReview,
        string submitAction)
    {
        var userId = User.GetUserId();

        // CreatedBy is supplied by the application,
        // not entered through the form.
        ModelState.Remove(
            nameof(CriticReview.CreatedBy));

        // Make sure the movie exists.
        var movie = await _context.Movies
            .FindAsync(criticReview.MovieId);

        if (movie == null)
        {
            return NotFound();
        }

        criticReview.Movie = movie;

        // Prevent one critic from reviewing
        // the same movie more than once.
        var reviewExists =
            await _context.CriticReviews
                .AnyAsync(
                    review =>
                        review.MovieId ==
                            criticReview.MovieId &&
                        review.CreatedBy == userId);

        if (reviewExists)
        {
            ModelState.AddModelError(
                string.Empty,
                "You already have a review for this movie.");
        }

        if (!ModelState.IsValid)
        {
            return View(criticReview);
        }

        // Store the logged-in critic's Auth0 user ID.
        criticReview.CreatedBy = userId;

        // Store the creation date.
        criticReview.CreatedDate = DateTime.Now;

        // Publish only when the Publish button is clicked.
        criticReview.IsPublished =
            submitAction == "publish";

        _context.CriticReviews.Add(criticReview);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // ==================== EDIT ====================

    // GET: Display a draft review for editing.
    public async Task<IActionResult> Edit(int id)
    {
        var userId = User.GetUserId();

        var criticReview =
            await _context.CriticReviews
                .Include(review => review.Movie)
                .FirstOrDefaultAsync(
                    review =>
                        review.Id == id &&
                        review.CreatedBy == userId);

        if (criticReview == null)
        {
            return NotFound();
        }

        // Published reviews cannot be edited.
        if (criticReview.IsPublished)
        {
            TempData["ReviewMessage"] =
                "Published reviews cannot be edited.";

            return RedirectToAction(nameof(Index));
        }

        return View(criticReview);
    }


    // POST: Update a draft review.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("Id,MovieId,Description,Rating")]
        CriticReview criticReview,
        string submitAction)
    {
        if (id != criticReview.Id)
        {
            return BadRequest();
        }

        var userId = User.GetUserId();

        // Only retrieve a review belonging to this critic.
        var existingReview =
            await _context.CriticReviews
                .FirstOrDefaultAsync(
                    review =>
                        review.Id == id &&
                        review.CreatedBy == userId);

        if (existingReview == null)
        {
            return NotFound();
        }

        // Published reviews cannot be changed.
        if (existingReview.IsPublished)
        {
            return Forbid();
        }

        ModelState.Remove(
            nameof(CriticReview.CreatedBy));

        if (!ModelState.IsValid)
        {
            criticReview.Movie =
                await _context.Movies.FindAsync(
                    criticReview.MovieId)
                ?? new Movie();

            return View(criticReview);
        }

        // Update only values the critic is allowed to change.
        existingReview.Description =
            criticReview.Description;

        existingReview.Rating =
            criticReview.Rating;

        // Publishing locks the review.
        if (submitAction == "publish")
        {
            existingReview.IsPublished = true;
        }

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }


    // ==================== DELETE ====================

    // GET: Display confirmation before deleting a draft.
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        var criticReview =
            await _context.CriticReviews
                .Include(review => review.Movie)
                .FirstOrDefaultAsync(
                    review =>
                        review.Id == id &&
                        review.CreatedBy == userId);

        if (criticReview == null)
        {
            return NotFound();
        }

        // Published reviews cannot be deleted.
        if (criticReview.IsPublished)
        {
            TempData["ReviewMessage"] =
                "Published reviews cannot be deleted.";

            return RedirectToAction(nameof(Index));
        }

        return View(criticReview);
    }


    // POST: Delete the logged-in critic's draft.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.GetUserId();

        var criticReview =
            await _context.CriticReviews
                .FirstOrDefaultAsync(
                    review =>
                        review.Id == id &&
                        review.CreatedBy == userId);

        if (criticReview == null)
        {
            return NotFound();
        }

        // Published reviews are locked.
        if (criticReview.IsPublished)
        {
            return Forbid();
        }

        _context.CriticReviews.Remove(criticReview);

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}