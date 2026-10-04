using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models;

public class Movie : IValidatableObject
{
    // SQL Server generates the movie ID.
    public int Id { get; set; }

    // Titles can legitimately contain numbers.
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = string.Empty;

    [Required]
    [StringLength(2000)]
    public string Synopsis { get; set; } = string.Empty;

    // Nullable strings allow optional information to be left blank.
    [StringLength(200)]
    public string? Director { get; set; }

    [StringLength(300)]
    public string? Producer { get; set; }

    [StringLength(300)]
    public string? Screenwriter { get; set; }

    [StringLength(200)]
    public string? Distributor { get; set; }

    [StringLength(200)]
    [Display(Name = "Production company")]
    public string? ProductionCompany { get; set; }

    // Accept only the movie and TV ratings supported by this app.
    [Required]
    [StringLength(15)]
    [Display(Name = "Content rating")]
    [RegularExpression(
        @"^(G|PG|PG-13|R|NC-17|NR|TV-Y|TV-Y7|TV-G|TV-PG|TV-14|TV-MA)$",
        ErrorMessage = "Choose a supported rating, such as PG-13, R or TV-PG.")]
    public string Rating { get; set; } = string.Empty;

    // Genre names allow letters and separators, but no numbers.
    [Required]
    [StringLength(60)]
    [RegularExpression(
        @"^[A-Za-z]+(?:[ ,'-]+[A-Za-z]+)*$",
        ErrorMessage = "Enter genre names such as Comedy, Romance or Sci-Fi.")]
    public string Genre { get; set; } = string.Empty;

    // Language names allow letters and separators, but no numbers.
    [StringLength(100)]
    [Display(Name = "Original language")]
    [RegularExpression(
        @"^[A-Za-z]+(?:[ ,'-]+[A-Za-z]+)*$",
        ErrorMessage = "Enter language names using letters, for example English or French.")]
    public string? OriginalLanguage { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Theatre release date")]
    public DateTime TheaterReleaseDate { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Streaming release date")]
    public DateTime? StreamingReleaseDate { get; set; }

    // int rejects decimals; Range checks the duration.
    [Range(1, 600,
        ErrorMessage = "Enter a runtime between 1 and 600 whole minutes.")]
    [Display(Name = "Runtime (minutes)")]
    public int RuntimeMinutes { get; set; }

    // Numbers are valid here, for example Dolby Digital 5.1.
    [StringLength(100)]
    [Display(Name = "Sound mix")]
    public string? SoundMix { get; set; }

    // Accept formats such as 1.85:1 or 16:9.
    [StringLength(30)]
    [Display(Name = "Aspect ratio")]
    [RegularExpression(
        @"^[1-9][0-9]*(?:\.[0-9]+)?:[1-9][0-9]*(?:\.[0-9]+)?$",
        ErrorMessage = "Enter an aspect ratio such as 1.85:1 or 16:9.")]
    public string? AspectRatio { get; set; }

    // Accept a simple image filename, without folders or a URL.
    [StringLength(150)]
    [Display(Name = "Image filename")]
    [RegularExpression(
        @"^[A-Za-z0-9][A-Za-z0-9_-]*\.(?:[pP][nN][gG]|[jJ][pP][gG]|[jJ][pP][eE][gG]|[wW][eE][bB][pP])$",
        ErrorMessage = "Use a filename such as home-alone-4.png or movie.jpg.")]
    public string? ImageFilename { get; set; }

    // These additional checks run on the server after submission.
    public IEnumerable<ValidationResult> Validate(
        ValidationContext validationContext)
    {
        // Reject numeric-only text while allowing company names with digits.
        var textFields = new (string? Value, string Property, string Label)[]
        {
            (Synopsis, nameof(Synopsis), "Synopsis"),
            (Director, nameof(Director), "Director"),
            (Producer, nameof(Producer), "Producer"),
            (Screenwriter, nameof(Screenwriter), "Screenwriter"),
            (Distributor, nameof(Distributor), "Distributor"),
            (ProductionCompany, nameof(ProductionCompany), "Production company"),
            (OriginalLanguage, nameof(OriginalLanguage), "Original language"),
            (SoundMix, nameof(SoundMix), "Sound mix")
        };

        foreach (var field in textFields)
        {
            if (!string.IsNullOrWhiteSpace(field.Value)
                && !field.Value.Any(char.IsLetter))
            {
                yield return new ValidationResult(
                    $"{field.Label} must contain letters, not just numbers or symbols.",
                    new[] { field.Property });
            }
        }

        // Check release dates against the range chosen for this app.
        // This validation allows release dates from January 1, 1888 through December 31, 2100, inclusive
        var earliestDate = new DateTime(1888, 1, 1);
        var latestDate = new DateTime(2100, 12, 31);

        if (TheaterReleaseDate.Date < earliestDate
            || TheaterReleaseDate.Date > latestDate)
        {
            yield return new ValidationResult(
                "Enter a theatre release date between 1888 and 2100.",
                [nameof(TheaterReleaseDate)]);
        }

        if (StreamingReleaseDate.HasValue
            && (StreamingReleaseDate.Value.Date < earliestDate
                || StreamingReleaseDate.Value.Date > latestDate))
        {
            yield return new ValidationResult(
                "Enter a streaming release date between 1888 and 2100.",
                 [nameof(StreamingReleaseDate)]);
        }
    }
}