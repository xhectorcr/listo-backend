using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ListoAPI.Aplication.Migrations
{
    /// <inheritdoc />
    public partial class AddPinAndEstadoSesion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "estado_sesion",
                table: "Usuario",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pin_temporal",
                table: "Usuario",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "estado_sesion",
                table: "Usuario");

            migrationBuilder.DropColumn(
                name: "pin_temporal",
                table: "Usuario");
        }
    }
}
