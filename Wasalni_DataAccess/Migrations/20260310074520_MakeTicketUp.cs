using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wasalni_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class MakeTicketUp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "QRstatus",
                table: "Tickets",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "QRstatus",
                table: "Tickets");
        }
    }
}
