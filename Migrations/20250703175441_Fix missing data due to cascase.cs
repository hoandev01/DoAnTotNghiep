using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class Fixmissingdataduetocascase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Flocks_Cages_CageId",
                table: "Flocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Flocks_Categories_CategoryId",
                table: "Flocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Flocks_FlockId",
                table: "Products");

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
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flocks_Cages_CageId",
                table: "Flocks",
                column: "CageId",
                principalTable: "Cages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flocks_Categories_CategoryId",
                table: "Flocks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Flocks_Categories_CategoryId1",
                table: "Flocks",
                column: "CategoryId1",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Flocks_FlockId",
                table: "Products",
                column: "FlockId",
                principalTable: "Flocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Trackings_Flocks_FlockId1",
                table: "Trackings",
                column: "FlockId1",
                principalTable: "Flocks",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems");

            migrationBuilder.DropForeignKey(
                name: "FK_Flocks_Cages_CageId",
                table: "Flocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Flocks_Categories_CategoryId",
                table: "Flocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Flocks_Categories_CategoryId1",
                table: "Flocks");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Flocks_FlockId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Trackings_Flocks_FlockId1",
                table: "Trackings");

            migrationBuilder.DropIndex(
                name: "IX_Trackings_FlockId1",
                table: "Trackings");

            migrationBuilder.DropIndex(
                name: "IX_Flocks_CategoryId1",
                table: "Flocks");

            migrationBuilder.DropColumn(
                name: "FlockId1",
                table: "Trackings");

            migrationBuilder.DropColumn(
                name: "CategoryId1",
                table: "Flocks");

            migrationBuilder.AddForeignKey(
                name: "FK_CartItems_Products_ProductId",
                table: "CartItems",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flocks_Cages_CageId",
                table: "Flocks",
                column: "CageId",
                principalTable: "Cages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Flocks_Categories_CategoryId",
                table: "Flocks",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Flocks_FlockId",
                table: "Products",
                column: "FlockId",
                principalTable: "Flocks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
