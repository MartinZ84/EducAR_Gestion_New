using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducAR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddEvaluacionesYNotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Evaluaciones",
                columns: table => new
                {
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    IdMateria = table.Column<int>(type: "int", nullable: false),
                    IdPeriodoEvaluacion = table.Column<int>(type: "int", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Evaluaciones", x => x.IdEvaluacion);
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Cursos_IdCurso",
                        column: x => x.IdCurso,
                        principalTable: "Cursos",
                        principalColumn: "IdCurso");
                    table.ForeignKey(
                        name: "FK_Evaluaciones_Materias_IdMateria",
                        column: x => x.IdMateria,
                        principalTable: "Materias",
                        principalColumn: "IdMateria");
                    table.ForeignKey(
                        name: "FK_Evaluaciones_PeriodosEvaluacion_IdPeriodoEvaluacion",
                        column: x => x.IdPeriodoEvaluacion,
                        principalTable: "PeriodosEvaluacion",
                        principalColumn: "IdPeriodoEvaluacion");
                });

            migrationBuilder.CreateTable(
                name: "NotasEvaluacion",
                columns: table => new
                {
                    IdNotaEvaluacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEvaluacion = table.Column<int>(type: "int", nullable: false),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    Valor = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotasEvaluacion", x => x.IdNotaEvaluacion);
                    table.ForeignKey(
                        name: "FK_NotasEvaluacion_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK_NotasEvaluacion_Evaluaciones_IdEvaluacion",
                        column: x => x.IdEvaluacion,
                        principalTable: "Evaluaciones",
                        principalColumn: "IdEvaluacion");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdCurso",
                table: "Evaluaciones",
                column: "IdCurso");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdMateria",
                table: "Evaluaciones",
                column: "IdMateria");

            migrationBuilder.CreateIndex(
                name: "IX_Evaluaciones_IdPeriodoEvaluacion",
                table: "Evaluaciones",
                column: "IdPeriodoEvaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_NotasEvaluacion_IdAlumno",
                table: "NotasEvaluacion",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_NotasEvaluacion_IdEvaluacion_IdAlumno",
                table: "NotasEvaluacion",
                columns: new[] { "IdEvaluacion", "IdAlumno" },
                unique: true);

            migrationBuilder.Sql(@"
                SELECT IdAlumno, IdMateria, IdPeriodoEvaluacion, IdCurso, ValorCalificacion, Fecha, FechaInicio
                INTO #CalificacionesPrevias
                FROM (
                    SELECT c.IdAlumno, c.IdMateria, c.IdPeriodoEvaluacion,
                           m.IdCurso, c.ValorCalificacion, c.Fecha, p.FechaInicio,
                           ROW_NUMBER() OVER (
                               PARTITION BY c.IdAlumno, c.IdMateria, c.IdPeriodoEvaluacion
                               ORDER BY c.Fecha DESC, c.IdCalificacion DESC
                           ) AS fila
                    FROM Calificaciones c
                    JOIN PeriodosEvaluacion p ON p.IdPeriodoEvaluacion = c.IdPeriodoEvaluacion
                    CROSS APPLY (
                        SELECT TOP 1 m.IdCurso
                        FROM Matriculas m
                        WHERE m.IdAlumno = c.IdAlumno
                          AND m.IdCicloLectivo = p.IdCicloLectivo
                        ORDER BY CASE WHEN m.Estado = 0 THEN 0 ELSE 1 END,
                                 m.FechaMatricula DESC
                    ) m
                    WHERE c.Activo = 1
                ) previas
                WHERE fila = 1;

                INSERT INTO Evaluaciones (IdCurso, IdMateria, IdPeriodoEvaluacion, Titulo, Fecha, Activo)
                SELECT IdCurso, IdMateria, IdPeriodoEvaluacion,
                       N'Calificación anterior', MIN(FechaInicio), 1
                FROM #CalificacionesPrevias
                GROUP BY IdCurso, IdMateria, IdPeriodoEvaluacion;

                INSERT INTO NotasEvaluacion (IdEvaluacion, IdAlumno, Valor, FechaAct)
                SELECT e.IdEvaluacion, p.IdAlumno, p.ValorCalificacion, GETDATE()
                FROM #CalificacionesPrevias p
                JOIN Evaluaciones e ON e.IdCurso = p.IdCurso
                                   AND e.IdMateria = p.IdMateria
                                   AND e.IdPeriodoEvaluacion = p.IdPeriodoEvaluacion
                                   AND e.Titulo = N'Calificación anterior';

                DROP TABLE #CalificacionesPrevias;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotasEvaluacion");

            migrationBuilder.DropTable(
                name: "Evaluaciones");
        }
    }
}
