using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;


namespace MoviesAdmin.Models;

public class Genre
{
    // Primary key.
    public int Id { get; set; }

    [Required]
    [StringLength(60)]
    public string Title { get; set; } = string.Empty;

    // One genre can have many movies.
    [ValidateNever]
    public List<Movie> Movies { get; set; }
        = new List<Movie>();
}