using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChickenF.Migrations
{
    /// <inheritdoc />
    public partial class CleanFlockTrackingRelationFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Không xóa foreign key vì nó đã bị xóa
            // Không xóa index vì nó đã bị xóa (nó gây lỗi)

            // migrationBuilder.DropForeignKey(
            //     name: "FK_Trackings_Flocks_FlockId1",
            //     table: "Trackings");

            // migrationBuilder.DropIndex(
            //     name: "IX_Trackings_FlockId1",
            //     table: "Trackings");

            // Chỉ cần xóa cột FlockId1 nếu còn tồn tại
            migrationBuilder.Sql(@"
        IF EXISTS (SELECT 1 FROM sys.columns 
                   WHERE Name = N'FlockId1' 
                   AND Object_ID = Object_ID(N'dbo.Trackings'))
        BEGIN
            ALTER TABLE [Trackings] DROP COLUMN [FlockId1]
        END
    ");
        }




        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
