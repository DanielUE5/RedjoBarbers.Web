using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedjoBarbers.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class EntityClassBarberUpdated : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FacebookUrl",
                table: "Barber",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "InstagramUrl",
                table: "Barber",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PhoneNumber",
                table: "Barber",
                type: "int",
                maxLength: 20,
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FacebookUrl",
                table: "Barber");

            migrationBuilder.DropColumn(
                name: "InstagramUrl",
                table: "Barber");

            migrationBuilder.DropColumn(
                name: "PhoneNumber",
                table: "Barber");
        }
    }
}
