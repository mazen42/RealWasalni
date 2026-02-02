using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wasalni_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class repairetherelationbetweenuserandpassengertables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Passengers_ApplicationUserId",
                table: "Passengers");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_ApplicationUserId",
                table: "Passengers",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Passengers_ApplicationUserId",
                table: "Passengers");

            migrationBuilder.CreateIndex(
                name: "IX_Passengers_ApplicationUserId",
                table: "Passengers",
                column: "ApplicationUserId",
                unique: true);
        }
    }
}
