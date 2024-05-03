using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace APIDatosMeteorologicos.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RegistrosMeteorologicos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Temperatura = table.Column<double>(type: "float", nullable: false),
                    Humedad = table.Column<double>(type: "float", nullable: false),
                    VelocidadViento = table.Column<double>(type: "float", nullable: false),
                    Precipitacion = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RegistrosMeteorologicos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RegistrosMeteorologicos");
        }
    }
}
