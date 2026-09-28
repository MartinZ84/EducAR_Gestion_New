using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducAR.API.Migrations
{
    /// <inheritdoc />
    public partial class CompletarEvaluaciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Evaluaciones_IdCurso",
                table: "Evaluaciones");

            migrationBuilder.AddColumn<string>(
                name: "Descripcion",
                table: "Evaluaciones",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Temario",
                table: "Evaluaciones",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "GeneradaPorEvaluaciones",
                table: "Calificaciones",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddCheckConstraint(
                name: "CK_NotasEvaluacion_Valor",
                table: "NotasEvaluacion",
                sql: "[Valor] >= 1 AND [Valor] <= 10");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdCurso_IdMateria_IdPeriodoEvaluacion_Activo",
                table: "Evaluaciones",
                columns: new[] { "IdCurso", "IdMateria", "IdPeriodoEvaluacion", "Activo" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_NotasEvaluacion_Valor",
                table: "NotasEvaluacion");

            migrationBuilder.DropIndex(
                name: "IX_Evaluaciones_IdCurso_IdMateria_IdPeriodoEvaluacion_Activo",
                table: "Evaluaciones");

            migrationBuilder.DropColumn(
                name: "Descripcion",
                table: "Evaluaciones");

            migrationBuilder.DropColumn(
                name: "Temario",
                table: "Evaluaciones");

            migrationBuilder.DropColumn(
                name: "GeneradaPorEvaluaciones",
                table: "Calificaciones");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdCurso",
                table: "Evaluaciones",
                column: "IdCurso");
        }
    }
}
