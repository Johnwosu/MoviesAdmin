using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models;

public class Movie
{
    public int Id { get; set; }

    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Synopsis { get; set; } = string.Empty;

    public string Director { get; set; } = string.Empty;

    public string Producer { get; set; } = string.Empty;

    public string Screenwriter { get; set; } = string.Empty;

    public string Distributor { get; set; } = string.Empty;

    // Display names make labels readable without renaming properties.
    [Display(Name = "Production company")]
    public string ProductionCompany { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    [Display(Name = "Content rating")]
    public string Rating { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Genre { get; set; } = string.Empty;

    [Display(Name = "Original language")]
    public string OriginalLanguage { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Theatre release date")]
    public DateTime TheaterReleaseDate { get; set; }

    // The question mark allows a movie to have no streaming release date.
    [DataType(DataType.Date)]
    [Display(Name = "Streaming release date")]
    public DateTime? StreamingReleaseDate { get; set; }

    // Store the total runtime in minutes; convert it for display in views.
    [Range(1, 600)]
    [Display(Name = "Runtime (minutes)")]
    public int RuntimeMinutes { get; set; }

    [Display(Name = "Sound mix")]
    public string SoundMix { get; set; } = string.Empty;

    [Display(Name = "Aspect ratio")]
    public string AspectRatio { get; set; } = string.Empty;

    [Display(Name = "Image filename")]
    public string ImageFilename { get; set; } = string.Empty;
}