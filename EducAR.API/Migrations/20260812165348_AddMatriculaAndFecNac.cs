using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducAR.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMatriculaAndFecNac : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoCursos");

            migrationBuilder.DropIndex(
                name: "IX_Alumnos_IdEscuela",
                table: "Alumnos");

            migrationBuilder.AddColumn<DateOnly>(
                name: "FecNac",
                table: "Alumnos",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    IdMatricula = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEscuela = table.Column<int>(type: "int", nullable: false),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    IdCicloLectivo = table.Column<int>(type: "int", nullable: false),
                    FechaMatricula = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaBaja = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Estado = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.IdMatricula);
                    table.ForeignKey(
                        name: "FK_Matriculas_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK_Matriculas_CiclosLectivos_IdCicloLectivo",
                        column: x => x.IdCicloLectivo,
                        principalTable: "CiclosLectivos",
                        principalColumn: "IdCicloLectivo");
                    table.ForeignKey(
                        name: "FK_Matriculas_Cursos_IdCurso",
                        column: x => x.IdCurso,
                        principalTable: "Cursos",
                        principalColumn: "IdCurso");
                    table.ForeignKey(
                        name: "FK_Matriculas_Escuelas_IdEscuela",
                        column: x => x.IdEscuela,
                        principalTable: "Escuelas",
                        principalColumn: "IdEscuela");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_IdEscuela_FechaCrea",
                table: "Alumnos",
                columns: new[] { "IdEscuela", "FechaCrea" });

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_IdAlumno_IdCicloLectivo",
                table: "Matriculas",
                columns: new[] { "IdAlumno", "IdCicloLectivo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_IdCicloLectivo",
                table: "Matriculas",
                column: "IdCicloLectivo");

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_IdCurso_Estado",
                table: "Matriculas",
                columns: new[] { "IdCurso", "Estado" });

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_IdEscuela",
                table: "Matriculas",
                column: "IdEscuela");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropIndex(
                name: "IX_Alumnos_IdEscuela_FechaCrea",
                table: "Alumnos");

            migrationBuilder.DropColumn(
                name: "FecNac",
                table: "Alumnos");

            migrationBuilder.CreateTable(
                name: "AlumnoCursos",
                columns: table => new
                {
                    IdAlumnoCurso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoCursos", x => x.IdAlumnoCurso);
                    table.ForeignKey(
                        name: "FK_AlumnoCursos_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK_AlumnoCursos_Cursos_IdCurso",
                        column: x => x.IdCurso,
                        principalTable: "Cursos",
                        principalColumn: "IdCurso");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_IdEscuela",
                table: "Alumnos",
                column: "IdEscuela");

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoCursos_IdAlumno_IdCurso",
                table: "AlumnoCursos",
                columns: new[] { "IdAlumno", "IdCurso" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoCursos_IdCurso",
                table: "AlumnoCursos",
                column: "IdCurso");
        }
    }
}
