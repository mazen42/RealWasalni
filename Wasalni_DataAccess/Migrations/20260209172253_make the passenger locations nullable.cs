using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wasalni_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class makethepassengerlocationsnullable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ToLocation_Longitude",
                table: "Passengers",
                type: "float(9)",
                precision: 9,
                scale: 4,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 4);

            migrationBuilder.AlterColumn<double>(
                name: "ToLocation_Latitude",
                table: "Passengers",
                type: "float(9)",
                precision: 9,
                scale: 4,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 4);

            migrationBuilder.AlterColumn<double>(
                name: "FromLocation_Longitude",
                table: "Passengers",
                type: "float(9)",
                precision: 9,
                scale: 4,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 4);

            migrationBuilder.AlterColumn<double>(
                name: "FromLocation_Latitude",
                table: "Passengers",
                type: "float(9)",
                precision: 9,
                scale: 4,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 4);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "ToLocation_Longitude",
                table: "Passengers",
                type: "float(9)",
                precision: 9,
                scale: 4,
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ToLocation_Latitude",
                table: "Passengers",
                type: "float(9)",
                precision: 9,
                scale: 4,
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FromLocation_Longitude",
                table: "Passengers",
                type: "float(9)",
                precision: 9,
                scale: 4,
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "FromLocation_Latitude",
                table: "Passengers",
                type: "float(9)",
                precision: 9,
                scale: 4,
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float(9)",
                oldPrecision: 9,
                oldScale: 4,
                oldNullable: true);
        }
    }
}
