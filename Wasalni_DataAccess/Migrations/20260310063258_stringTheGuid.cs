using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wasalni_DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class stringTheGuid : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ticketguid",
                table: "Tickets",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ticketguid",
                table: "Tickets");
        }
    }
}
