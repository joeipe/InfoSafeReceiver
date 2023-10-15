using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InfoSafeReceiver.Data.Migrations
{
    /// <inheritdoc />
    public partial class RefId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RefId",
                schema: "Receiver",
                table: "Contacts",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefId",
                schema: "Receiver",
                table: "Contacts");
        }
    }
}