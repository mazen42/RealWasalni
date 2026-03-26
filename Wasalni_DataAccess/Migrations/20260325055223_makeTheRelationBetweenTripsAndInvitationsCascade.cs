using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wasalni_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class makeTheRelationBetweenTripsAndInvitationsCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_BusTrips_BusTripId",
                table: "Invitations");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_BusTrips_BusTripId",
                table: "Invitations",
                column: "BusTripId",
                principalTable: "BusTrips",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_BusTrips_BusTripId",
                table: "Invitations");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_BusTrips_BusTripId",
                table: "Invitations",
                column: "BusTripId",
                principalTable: "BusTrips",
                principalColumn: "Id");
        }
    }
}
