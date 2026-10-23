using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models;

public class AudienceRating
{
    // Primary key.
    public int Id { get; set; }

    [Required]
    [StringLength(15)]
    [Display(Name = "Content rating")]
    public string Title { get; set; } = string.Empty;

    // One audience rating can have many movies.
    [ValidateNever]
    public List<Movie> Movies { get; set; }
        = new List<Movie>();
}