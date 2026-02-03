using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RedjoBarbers.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialNewEntityClassBarber : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BarberId",
                table: "Appointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Barber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    PhotoUrl = table.Column<string>(type: "nvarchar(2048)", maxLength: 2048, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Barber", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BarberBarberService",
                columns: table => new
                {
                    BarberServicesId = table.Column<int>(type: "int", nullable: false),
                    BarbersId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarberBarberService", x => new { x.BarberServicesId, x.BarbersId });
                    table.ForeignKey(
                        name: "FK_BarberBarberService_BarberServices_BarberServicesId",
                        column: x => x.BarberServicesId,
                        principalTable: "BarberServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BarberBarberService_Barber_BarbersId",
                        column: x => x.BarbersId,
                        principalTable: "Barber",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_BarberId",
                table: "Appointments",
                column: "BarberId");

            migrationBuilder.CreateIndex(
                name: "IX_BarberBarberService_BarbersId",
                table: "BarberBarberService",
                column: "BarbersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Barber_BarberId",
                table: "Appointments",
                column: "BarberId",
                principalTable: "Barber",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Barber_BarberId",
                table: "Appointments");

            migrationBuilder.DropTable(
                name: "BarberBarberService");

            migrationBuilder.DropTable(
                name: "Barber");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_BarberId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "BarberId",
                table: "Appointments");
        }
    }
}
