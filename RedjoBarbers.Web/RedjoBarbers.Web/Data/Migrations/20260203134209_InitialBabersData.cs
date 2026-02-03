using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedjoBarbers.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialBabersData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Barbers_PhoneNumber",
                table: "Barbers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Barbers_PhoneNumber",
                table: "Barbers",
                column: "PhoneNumber",
                unique: true);
        }
    }
}
