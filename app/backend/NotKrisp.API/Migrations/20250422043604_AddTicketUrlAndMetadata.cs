using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotKrisp.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketUrlAndMetadata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Metadata",
                table: "Tickets",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Tickets",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Metadata",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Tickets");
        }
    }
}
