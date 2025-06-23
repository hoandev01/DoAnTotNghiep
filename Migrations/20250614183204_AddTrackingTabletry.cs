using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class AddTrackingTabletry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Trackings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlockId = table.Column<int>(type: "int", nullable: false),
                    TrackingDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    HealthStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Temperature = table.Column<float>(type: "real", nullable: true),
                    Humidity = table.Column<float>(type: "real", nullable: true),
                    FeedAmount = table.Column<float>(type: "real", nullable: true),
                    DeathCount = table.Column<int>(type: "int", nullable: true),
                    Note = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Trackings_Flocks_FlockId",
                        column: x => x.FlockId,
                        principalTable: "Flocks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trackings_FlockId",
                table: "Trackings",
                column: "FlockId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Trackings");
        }
    }
}
