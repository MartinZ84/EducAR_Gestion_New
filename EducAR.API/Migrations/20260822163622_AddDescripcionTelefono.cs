using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducAR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDescripcionTelefono : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>("Des", "TelefonosContacto", type: "nvarchar(100)", maxLength: 100, nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn("Des", "TelefonosContacto");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesBoletines_IdBoletin",
                table: "DetallesBoletines",
                column: "IdBoletin");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdAlumno",
                table: "Calificaciones",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_Boletines_IdAlumno",
                table: "Boletines",
                column: "IdAlumno");
        }
    }
}
