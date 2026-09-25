namespace MoviesAdmin.Models;

public class Movie
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public string Synopsis { get; set; } = string.Empty;

    public string Director { get; set; } = string.Empty;

    public string Producer { get; set; } = string.Empty;

    public string Screenwriter { get; set; } = string.Empty;

    public string Distributor { get; set; } = string.Empty;

    public string ProductionCompany { get; set; } = string.Empty;

    public string Rating { get; set; } = string.Empty;

    public string Genre { get; set; } = string.Empty;

    public string OriginalLanguage { get; set; } = string.Empty;

    public DateTime TheaterReleaseDate { get; set; }

    public DateTime StreamingReleaseDate { get; set; }

    public int RuntimeMinutes { get; set; }

    public string SoundMix { get; set; } = string.Empty;

    public string AspectRatio { get; set; } = string.Empty;

    public string ImageFilename { get; set; } = string.Empty;
}