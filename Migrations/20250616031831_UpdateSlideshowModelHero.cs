using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSlideshowModelHero : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Slides",
                columns: new[] { "Id", "ButtonLink", "ButtonText", "DisplayOrder", "ImageUrl", "IsActive", "Subtitle", "Title" },
                values: new object[,]
                {
                    { 1, "/about", "Learn More", 1, "/Image/slide1.jpg", true, "Digital Transformation for Poultry", "Welcome to Smart Chicken Farm" },
                    { 2, "/shop", "Start Now", 2, "/Image/slide2.jpg", true, "All-in-one system for farm owners", "Manage & Sell Easily" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Slides",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
