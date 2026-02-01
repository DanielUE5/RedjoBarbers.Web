using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedjoBarbers.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class BarberServicesDataUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_BarberServices_Name",
                table: "BarberServices",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BarberServices_Name",
                table: "BarberServices");
        }
    }
}
