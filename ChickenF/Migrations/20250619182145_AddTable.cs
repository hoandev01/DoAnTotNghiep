using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class AddTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedAmount",
                table: "Trackings");

            migrationBuilder.AddColumn<int>(
                name: "FeedCost",
                table: "Trackings",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "FlockReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlockId = table.Column<int>(type: "int", nullable: false),
                    TotalRaised = table.Column<int>(type: "int", nullable: false),
                    TotalSold = table.Column<int>(type: "int", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalFeedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ReportGeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlockReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlockReports_Flocks_FlockId",
                        column: x => x.FlockId,
                        principalTable: "Flocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlockReports_FlockId",
                table: "FlockReports",
                column: "FlockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlockReports");

            migrationBuilder.DropColumn(
                name: "FeedCost",
                table: "Trackings");

            migrationBuilder.AddColumn<float>(
                name: "FeedAmount",
                table: "Trackings",
                type: "real",
                nullable: true);
        }
    }
}
