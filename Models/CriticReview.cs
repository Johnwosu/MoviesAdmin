using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models;

public class CriticReview
{
    public int Id { get; set; }

    // Foreign key to Movie.
    public int MovieId { get; set; }

    // Navigation property.
    [ValidateNever]
    public Movie Movie { get; set; } = null!;

    // Review text can contain up to 1000 characters.
    [Required]
    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    // Numerical rating from 0 to 5.
    [Range(0, 5)]
    public int Rating { get; set; }

    // Draft = false, Published = true.
    public bool IsPublished { get; set; }

    // Auth0 ID of the critic who created the review.
    public string CreatedBy { get; set; } = string.Empty;

    // Date the review was created.
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}