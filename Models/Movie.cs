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
    public string ProductionCompany { get; set; } = string.Empty;

    [Required]
    [StringLength(15)]
    public string Rating { get; set; } = string.Empty;

    [Required]
    [StringLength(60)]
    public string Genre { get; set; } = string.Empty;

    public string OriginalLanguage { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Theatre release date")]
    public DateTime TheaterReleaseDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Streaming release date")]
    public DateTime? StreamingReleaseDate { get; set; }  // Nullable DateTime for streaming release date

    [Range(1, 600)]
    [Display(Name = "Runtime (minutes)")]
    public int RuntimeMinutes { get; set; }

    public string SoundMix { get; set; } = string.Empty;
    public string AspectRatio { get; set; } = string.Empty;
    public string ImageFilename { get; set; } = string.Empty;
}