using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MoviesAdmin.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Synopsis = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Director = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Producer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Screenwriter = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Distributor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ProductionCompany = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rating = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: false),
                    Genre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    OriginalLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TheaterReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StreamingReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RuntimeMinutes = table.Column<int>(type: "int", nullable: false),
                    SoundMix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AspectRatio = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageFilename = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");
        }
    }
}
