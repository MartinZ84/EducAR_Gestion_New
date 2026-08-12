using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducAR.API.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Escuelas",
                columns: table => new
                {
                    IdEscuela = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Direccion = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Telefono = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Escuelas", x => x.IdEscuela);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    IdRol = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.IdRol);
                });

            migrationBuilder.CreateTable(
                name: "Alumnos",
                columns: table => new
                {
                    IdAlumno = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEscuela = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Dni = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Alumnos", x => x.IdAlumno);
                    table.ForeignKey(
                        name: "FK_Alumnos_Escuelas_IdEscuela",
                        column: x => x.IdEscuela,
                        principalTable: "Escuelas",
                        principalColumn: "IdEscuela");
                });

            migrationBuilder.CreateTable(
                name: "CiclosLectivos",
                columns: table => new
                {
                    IdCicloLectivo = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEscuela = table.Column<int>(type: "int", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CiclosLectivos", x => x.IdCicloLectivo);
                    table.ForeignKey(
                        name: "FK_CiclosLectivos_Escuelas_IdEscuela",
                        column: x => x.IdEscuela,
                        principalTable: "Escuelas",
                        principalColumn: "IdEscuela");
                });

            migrationBuilder.CreateTable(
                name: "Materias",
                columns: table => new
                {
                    IdMateria = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEscuela = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Materias", x => x.IdMateria);
                    table.ForeignKey(
                        name: "FK_Materias_Escuelas_IdEscuela",
                        column: x => x.IdEscuela,
                        principalTable: "Escuelas",
                        principalColumn: "IdEscuela");
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    IdUsuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRol = table.Column<int>(type: "int", nullable: false),
                    IdEscuela = table.Column<int>(type: "int", nullable: false),
                    Dni = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    HashContrasena = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    UltimoAcceso = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.IdUsuario);
                    table.ForeignKey(
                        name: "FK_Usuarios_Escuelas_IdEscuela",
                        column: x => x.IdEscuela,
                        principalTable: "Escuelas",
                        principalColumn: "IdEscuela");
                    table.ForeignKey(
                        name: "FK_Usuarios_Roles_IdRol",
                        column: x => x.IdRol,
                        principalTable: "Roles",
                        principalColumn: "IdRol");
                });

            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    IdCurso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdEscuela = table.Column<int>(type: "int", nullable: false),
                    IdCicloLectivo = table.Column<int>(type: "int", nullable: false),
                    Grado = table.Column<int>(type: "int", nullable: false),
                    Division = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Turno = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.IdCurso);
                    table.ForeignKey(
                        name: "FK_Cursos_CiclosLectivos_IdCicloLectivo",
                        column: x => x.IdCicloLectivo,
                        principalTable: "CiclosLectivos",
                        principalColumn: "IdCicloLectivo");
                    table.ForeignKey(
                        name: "FK_Cursos_Escuelas_IdEscuela",
                        column: x => x.IdEscuela,
                        principalTable: "Escuelas",
                        principalColumn: "IdEscuela");
                });

            migrationBuilder.CreateTable(
                name: "PeriodosEvaluacion",
                columns: table => new
                {
                    IdPeriodoEvaluacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdCicloLectivo = table.Column<int>(type: "int", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaInicio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaFin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeriodosEvaluacion", x => x.IdPeriodoEvaluacion);
                    table.ForeignKey(
                        name: "FK_PeriodosEvaluacion_CiclosLectivos_IdCicloLectivo",
                        column: x => x.IdCicloLectivo,
                        principalTable: "CiclosLectivos",
                        principalColumn: "IdCicloLectivo");
                });

            migrationBuilder.CreateTable(
                name: "Docentes",
                columns: table => new
                {
                    IdDocente = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Docentes", x => x.IdDocente);
                    table.ForeignKey(
                        name: "FK_Docentes_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Mensajes",
                columns: table => new
                {
                    IdMensaje = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuarioRemitente = table.Column<int>(type: "int", nullable: false),
                    IdUsuarioDestinat = table.Column<int>(type: "int", nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    MensajeTexto = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaEnvio = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Leido = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Mensajes", x => x.IdMensaje);
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_IdUsuarioDestinat",
                        column: x => x.IdUsuarioDestinat,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                    table.ForeignKey(
                        name: "FK_Mensajes_Usuarios_IdUsuarioRemitente",
                        column: x => x.IdUsuarioRemitente,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "Tutores",
                columns: table => new
                {
                    IdTutor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUsuario = table.Column<int>(type: "int", nullable: false),
                    EsResponsable = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tutores", x => x.IdTutor);
                    table.ForeignKey(
                        name: "FK_Tutores_Usuarios_IdUsuario",
                        column: x => x.IdUsuario,
                        principalTable: "Usuarios",
                        principalColumn: "IdUsuario");
                });

            migrationBuilder.CreateTable(
                name: "AlumnoCursos",
                columns: table => new
                {
                    IdAlumnoCurso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Boletines",
                columns: table => new
                {
                    IdBoletin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    IdPeriodoEvaluacion = table.Column<int>(type: "int", nullable: false),
                    ObservacionGeneral = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Boletines", x => x.IdBoletin);
                    table.ForeignKey(
                        name: "FK_Boletines_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK_Boletines_Cursos_IdCurso",
                        column: x => x.IdCurso,
                        principalTable: "Cursos",
                        principalColumn: "IdCurso");
                    table.ForeignKey(
                        name: "FK_Boletines_PeriodosEvaluacion_IdPeriodoEvaluacion",
                        column: x => x.IdPeriodoEvaluacion,
                        principalTable: "PeriodosEvaluacion",
                        principalColumn: "IdPeriodoEvaluacion");
                });

            migrationBuilder.CreateTable(
                name: "Calificaciones",
                columns: table => new
                {
                    IdCalificacion = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdMateria = table.Column<int>(type: "int", nullable: false),
                    IdPeriodoEvaluacion = table.Column<int>(type: "int", nullable: false),
                    ValorCalificacion = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    NivelCalificacion = table.Column<int>(type: "int", nullable: true),
                    Observacion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calificaciones", x => x.IdCalificacion);
                    table.ForeignKey(
                        name: "FK_Calificaciones_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK_Calificaciones_Materias_IdMateria",
                        column: x => x.IdMateria,
                        principalTable: "Materias",
                        principalColumn: "IdMateria");
                    table.ForeignKey(
                        name: "FK_Calificaciones_PeriodosEvaluacion_IdPeriodoEvaluacion",
                        column: x => x.IdPeriodoEvaluacion,
                        principalTable: "PeriodosEvaluacion",
                        principalColumn: "IdPeriodoEvaluacion");
                });

            migrationBuilder.CreateTable(
                name: "Asistencias",
                columns: table => new
                {
                    IdAsistencia = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDocente = table.Column<int>(type: "int", nullable: false),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    Fecha = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Presente = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Asistencias", x => x.IdAsistencia);
                    table.ForeignKey(
                        name: "FK_Asistencias_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK_Asistencias_Cursos_IdCurso",
                        column: x => x.IdCurso,
                        principalTable: "Cursos",
                        principalColumn: "IdCurso");
                    table.ForeignKey(
                        name: "FK_Asistencias_Docentes_IdDocente",
                        column: x => x.IdDocente,
                        principalTable: "Docentes",
                        principalColumn: "IdDocente");
                });

            migrationBuilder.CreateTable(
                name: "DocenteMateriaCursos",
                columns: table => new
                {
                    IdDocenteMateriaCurso = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdDocente = table.Column<int>(type: "int", nullable: false),
                    IdMateria = table.Column<int>(type: "int", nullable: false),
                    IdCurso = table.Column<int>(type: "int", nullable: false),
                    FechaAsignacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocenteMateriaCursos", x => x.IdDocenteMateriaCurso);
                    table.ForeignKey(
                        name: "FK_DocenteMateriaCursos_Cursos_IdCurso",
                        column: x => x.IdCurso,
                        principalTable: "Cursos",
                        principalColumn: "IdCurso");
                    table.ForeignKey(
                        name: "FK_DocenteMateriaCursos_Docentes_IdDocente",
                        column: x => x.IdDocente,
                        principalTable: "Docentes",
                        principalColumn: "IdDocente");
                    table.ForeignKey(
                        name: "FK_DocenteMateriaCursos_Materias_IdMateria",
                        column: x => x.IdMateria,
                        principalTable: "Materias",
                        principalColumn: "IdMateria");
                });

            migrationBuilder.CreateTable(
                name: "AlumnoTutores",
                columns: table => new
                {
                    IdAlumnoTutor = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAlumno = table.Column<int>(type: "int", nullable: false),
                    IdTutor = table.Column<int>(type: "int", nullable: false),
                    Parentesco = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EsResponsablePrinc = table.Column<bool>(type: "bit", nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlumnoTutores", x => x.IdAlumnoTutor);
                    table.ForeignKey(
                        name: "FK_AlumnoTutores_Alumnos_IdAlumno",
                        column: x => x.IdAlumno,
                        principalTable: "Alumnos",
                        principalColumn: "IdAlumno");
                    table.ForeignKey(
                        name: "FK_AlumnoTutores_Tutores_IdTutor",
                        column: x => x.IdTutor,
                        principalTable: "Tutores",
                        principalColumn: "IdTutor");
                });

            migrationBuilder.CreateTable(
                name: "DetallesBoletines",
                columns: table => new
                {
                    IdDetalleBoletin = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdBoletin = table.Column<int>(type: "int", nullable: false),
                    IdMateria = table.Column<int>(type: "int", nullable: false),
                    CalificacionFinal = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    ConceptoFinal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false),
                    FechaCrea = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaAct = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BoletinIdBoletin = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DetallesBoletines", x => x.IdDetalleBoletin);
                    table.ForeignKey(
                        name: "FK_DetallesBoletines_Boletines_BoletinIdBoletin",
                        column: x => x.BoletinIdBoletin,
                        principalTable: "Boletines",
                        principalColumn: "IdBoletin");
                    table.ForeignKey(
                        name: "FK_DetallesBoletines_Boletines_IdBoletin",
                        column: x => x.IdBoletin,
                        principalTable: "Boletines",
                        principalColumn: "IdBoletin");
                    table.ForeignKey(
                        name: "FK_DetallesBoletines_Materias_IdMateria",
                        column: x => x.IdMateria,
                        principalTable: "Materias",
                        principalColumn: "IdMateria");
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoCursos_IdAlumno_IdCurso",
                table: "AlumnoCursos",
                columns: new[] { "IdAlumno", "IdCurso" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoCursos_IdCurso",
                table: "AlumnoCursos",
                column: "IdCurso");

            migrationBuilder.CreateIndex(
                name: "IX_Alumnos_IdEscuela",
                table: "Alumnos",
                column: "IdEscuela");

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoTutores_IdAlumno_IdTutor",
                table: "AlumnoTutores",
                columns: new[] { "IdAlumno", "IdTutor" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlumnoTutores_IdTutor",
                table: "AlumnoTutores",
                column: "IdTutor");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdAlumno_IdCurso_Fecha",
                table: "Asistencias",
                columns: new[] { "IdAlumno", "IdCurso", "Fecha" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdCurso",
                table: "Asistencias",
                column: "IdCurso");

            migrationBuilder.CreateIndex(
                name: "IX_Asistencias_IdDocente",
                table: "Asistencias",
                column: "IdDocente");

            migrationBuilder.CreateIndex(
                name: "IX_Boletines_IdAlumno",
                table: "Boletines",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_Boletines_IdCurso",
                table: "Boletines",
                column: "IdCurso");

            migrationBuilder.CreateIndex(
                name: "IX_Boletines_IdPeriodoEvaluacion",
                table: "Boletines",
                column: "IdPeriodoEvaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdAlumno",
                table: "Calificaciones",
                column: "IdAlumno");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdMateria",
                table: "Calificaciones",
                column: "IdMateria");

            migrationBuilder.CreateIndex(
                name: "IX_Calificaciones_IdPeriodoEvaluacion",
                table: "Calificaciones",
                column: "IdPeriodoEvaluacion");

            migrationBuilder.CreateIndex(
                name: "IX_CiclosLectivos_IdEscuela",
                table: "CiclosLectivos",
                column: "IdEscuela");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_IdCicloLectivo",
                table: "Cursos",
                column: "IdCicloLectivo");

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_IdEscuela",
                table: "Cursos",
                column: "IdEscuela");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesBoletines_BoletinIdBoletin",
                table: "DetallesBoletines",
                column: "BoletinIdBoletin");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesBoletines_IdBoletin",
                table: "DetallesBoletines",
                column: "IdBoletin");

            migrationBuilder.CreateIndex(
                name: "IX_DetallesBoletines_IdMateria",
                table: "DetallesBoletines",
                column: "IdMateria");

            migrationBuilder.CreateIndex(
                name: "IX_DocenteMateriaCursos_IdCurso",
                table: "DocenteMateriaCursos",
                column: "IdCurso");

            migrationBuilder.CreateIndex(
                name: "IX_DocenteMateriaCursos_IdDocente_IdMateria_IdCurso",
                table: "DocenteMateriaCursos",
                columns: new[] { "IdDocente", "IdMateria", "IdCurso" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocenteMateriaCursos_IdMateria",
                table: "DocenteMateriaCursos",
                column: "IdMateria");

            migrationBuilder.CreateIndex(
                name: "IX_Docentes_IdUsuario",
                table: "Docentes",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Materias_IdEscuela",
                table: "Materias",
                column: "IdEscuela");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_IdUsuarioDestinat",
                table: "Mensajes",
                column: "IdUsuarioDestinat");

            migrationBuilder.CreateIndex(
                name: "IX_Mensajes_IdUsuarioRemitente",
                table: "Mensajes",
                column: "IdUsuarioRemitente");

            migrationBuilder.CreateIndex(
                name: "IX_PeriodosEvaluacion_IdCicloLectivo",
                table: "PeriodosEvaluacion",
                column: "IdCicloLectivo");

            migrationBuilder.CreateIndex(
                name: "IX_Tutores_IdUsuario",
                table: "Tutores",
                column: "IdUsuario",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdEscuela_NombreUsuario",
                table: "Usuarios",
                columns: new[] { "IdEscuela", "NombreUsuario" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_IdRol",
                table: "Usuarios",
                column: "IdRol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AlumnoCursos");

            migrationBuilder.DropTable(
                name: "AlumnoTutores");

            migrationBuilder.DropTable(
                name: "Asistencias");

            migrationBuilder.DropTable(
                name: "Calificaciones");

            migrationBuilder.DropTable(
                name: "DetallesBoletines");

            migrationBuilder.DropTable(
                name: "DocenteMateriaCursos");

            migrationBuilder.DropTable(
                name: "Mensajes");

            migrationBuilder.DropTable(
                name: "Tutores");

            migrationBuilder.DropTable(
                name: "Boletines");

            migrationBuilder.DropTable(
                name: "Docentes");

            migrationBuilder.DropTable(
                name: "Materias");

            migrationBuilder.DropTable(
                name: "Alumnos");

            migrationBuilder.DropTable(
                name: "Cursos");

            migrationBuilder.DropTable(
                name: "PeriodosEvaluacion");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropTable(
                name: "CiclosLectivos");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Escuelas");
        }
    }
}
