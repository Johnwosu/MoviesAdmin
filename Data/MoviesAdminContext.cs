using Microsoft.EntityFrameworkCore;
using MoviesAdmin.Models;

namespace MoviesAdmin.Data;

public class MoviesAdminContext : DbContext
{
    // Constructor that takes DbContextOptions and passes them to the base DbContext class
    public MoviesAdminContext(DbContextOptions<MoviesAdminContext> options)
        : base(options)
    {
    }

    // DbSet property for the Movie entity, representing the Movies table in the database
    public DbSet<Movie> Movies { get; set; } = default!; // ! only suppresses a compiler warning, EFCore supplies the movie set.

    // Represents the Genres table.
    public DbSet<Genre> Genres { get; set; } = default!;

    // Represents the AudienceRatings table.
    public DbSet<AudienceRating> AudienceRatings { get; set; } = default!;
}