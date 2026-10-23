using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace MoviesAdmin.Models
{
    public class Movie
    {
        // Primary key.
        public int Id { get; set; }


        // Foreign key for Genre.
        public int GenreId { get; set; }

        // Navigation property.
        [ValidateNever]
        public Genre Genre { get; set; } = null!;


        // Foreign key for Audience Rating.
        public int AudienceRatingId { get; set; }

        // Navigation property.
        [ValidateNever]
        public AudienceRating AudienceRating { get; set; } = null!;


        [StringLength(150)]
        [Required]
        public string Title { get; set; } = string.Empty;


        [StringLength(2000)]
        [Required]
        public string Synopsis { get; set; } = string.Empty;


        [Display(Name = "Runtime (minutes)")]
        [Range(1, 600)]
        [Required]
        public int RuntimeMinutes { get; set; }


        [DataType(DataType.Date)]
        [Display(Name = "Theatre release date")]
        [Required]
        public DateTime TheaterReleaseDate { get; set; }


        // URL to the movie poster stored in Blob Storage.
        public string ImageUrl { get; set; } = string.Empty;
    }
}