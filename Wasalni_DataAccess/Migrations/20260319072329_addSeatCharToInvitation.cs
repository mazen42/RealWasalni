using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wasalni_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class addSeatCharToInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "seatChar",
                table: "Invitations",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "seatChar",
                table: "Invitations");
        }
    }
}
