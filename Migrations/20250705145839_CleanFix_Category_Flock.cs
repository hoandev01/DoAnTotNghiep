using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class CleanFix_Category_Flock : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            
            

            

            

            
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FlockId1",
                table: "Trackings",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CategoryId1",
                table: "Flocks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Trackings_FlockId1",
                table: "Trackings",
                column: "FlockId1");

            migrationBuilder.CreateIndex(
                name: "IX_Flocks_CategoryId1",
                table: "Flocks",
                column: "CategoryId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Flocks_Categories_CategoryId1",
                table: "Flocks",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trackings_Flocks_FlockId1",
                table: "Trackings",
                column: "FlockId1",
                principalTable: "Flocks",
                principalColumn: "Id");
        }
    }
}
