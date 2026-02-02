using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wasalni_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addenumsdriver_trips_requestsandtrips_status : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "BusTrips");

            migrationBuilder.DropColumn(
                name: "IsOptimized",
                table: "BusTrips");

            migrationBuilder.AddColumn<int>(
                name: "RequestStatus",
                table: "DriverTripRequests",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TripStatus",
                table: "BusTrips",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RequestStatus",
                table: "DriverTripRequests");

            migrationBuilder.DropColumn(
                name: "TripStatus",
                table: "BusTrips");

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "BusTrips",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOptimized",
                table: "BusTrips",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
