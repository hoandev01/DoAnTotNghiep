using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSlideshowModelmore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "LinkUrl",
                table: "Slides",
                newName: "ButtonText");

            migrationBuilder.AddColumn<string>(
                name: "ButtonLink",
                table: "Slides",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Slides",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Slides",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ButtonLink",
                table: "Slides");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Slides");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Slides");

            migrationBuilder.RenameColumn(
                name: "ButtonText",
                table: "Slides",
                newName: "LinkUrl");
        }
    }
}
