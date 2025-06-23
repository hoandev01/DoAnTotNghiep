using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class RemoveFlockReport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlockReports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlockReports",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlockId = table.Column<int>(type: "int", nullable: false),
                    ReportContent = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReportGeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalFeedCost = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalRaised = table.Column<int>(type: "int", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalSold = table.Column<int>(type: "int", nullable: false)
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
    }
}
