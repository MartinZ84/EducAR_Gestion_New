using EducAR.API.Controllers;
using EducAR.API.Data;
using EducAR.API.Models;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class EvaluacionesControllerTests
{
    private readonly AppDbContext _context;
    private readonly EvaluacionesController _controller;

    public EvaluacionesControllerTests()
    {
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        _context.Escuelas.Add(new Escuela { IdEscuela = 1, Nombre = "Escuela", Direccion = "Calle 1" });
        _context.Roles.Add(new Rol { IdRol = 2, Nombre = "Docente" });
        _context.Usuarios.Add(new Usuario
        {
            IdUsuario = 2, IdRol = 2, IdEscuela = 1, Nombre = "Docente", Apellido = "Test",
            Email = "docente@test.com", NombreUsuario = "docente", HashContrasena = "hash"
        });
        _context.Docentes.Add(new Docente { IdDocente = 1, IdUsuario = 2 });
        _context.CiclosLectivos.Add(new CicloLectivo { IdCicloLectivo = 1, IdEscuela = 1, Anio = DateTime.Now.Year });
        _context.Cursos.Add(new Curso { IdCurso = 1, IdEscuela = 1, IdCicloLectivo = 1, Division = "A", Activo = true });
        _context.Materias.Add(new Materia { IdMateria = 1, IdEscuela = 1, Nombre = "Matemática" });
        _context.PeriodosEvaluacion.Add(new PeriodoEvaluacion
        {
            IdPeriodoEvaluacion = 1, IdCicloLectivo = 1, Nombre = "Primer período",
            FechaInicio = new DateTime(DateTime.Now.Year, 3, 1), FechaFin = new DateTime(DateTime.Now.Year, 12, 1)
        });
        _context.DocenteMateriaCursos.Add(new DocenteMateriaCurso
            { IdDocenteMateriaCurso = 1, IdDocente = 1, IdCurso = 1, IdMateria = 1 });
        _context.Alumnos.Add(new Alumno { IdAlumno = 1, IdEscuela = 1, Nombre = "Alumno", Apellido = "Test" });
        _context.Matriculas.Add(new Matricula
            { IdMatricula = 1, IdEscuela = 1, IdAlumno = 1, IdCurso = 1, IdCicloLectivo = 1, Estado = EstadoMatricula.Activa });
        _context.Evaluaciones.AddRange(
            new Evaluacion { IdEvaluacion = 1, IdCurso = 1, IdMateria = 1, IdPeriodoEvaluacion = 1, Titulo = "Prueba 1", Fecha = DateTime.Today },
            new Evaluacion { IdEvaluacion = 2, IdCurso = 1, IdMateria = 1, IdPeriodoEvaluacion = 1, Titulo = "Prueba 2", Fecha = DateTime.Today });
        _context.SaveChanges();

        _controller = new EvaluacionesController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "2"), new Claim("IdEscuela", "1")
                    }, "Test"))
                }
            }
        };
    }

    [Fact]
    public async Task GuardarNotas_DosEvaluaciones_CalculaPromedioFinal()
    {
        var primero = await _controller.GuardarNotas(1, new NotasEvaluacionGuardarDto
            { Notas = new() { new() { IdAlumno = 1, Valor = 7 } } });
        var segundo = await _controller.GuardarNotas(2, new NotasEvaluacionGuardarDto
            { Notas = new() { new() { IdAlumno = 1, Valor = 9 } } });

        primero.Should().BeOfType<OkObjectResult>();
        segundo.Should().BeOfType<OkObjectResult>();
        (await _context.Calificaciones.SingleAsync()).ValorCalificacion.Should().Be(8);
    }

    [Fact]
    public async Task BorrarNota_RecalculaPromedioYDesactivaFinalSinNotas()
    {
        await _controller.GuardarNotas(1, new NotasEvaluacionGuardarDto
            { Notas = new() { new() { IdAlumno = 1, Valor = 7 } } });
        await _controller.GuardarNotas(2, new NotasEvaluacionGuardarDto
            { Notas = new() { new() { IdAlumno = 1, Valor = 9 } } });

        await _controller.GuardarNotas(1, new NotasEvaluacionGuardarDto
            { Notas = new() { new() { IdAlumno = 1, Valor = null } } });
        (await _context.Calificaciones.SingleAsync()).ValorCalificacion.Should().Be(9);

        await _controller.GuardarNotas(2, new NotasEvaluacionGuardarDto
            { Notas = new() { new() { IdAlumno = 1, Valor = null } } });
        (await _context.Calificaciones.SingleAsync()).Activo.Should().BeFalse();
    }

    [Fact]
    public async Task GuardarNotas_AlumnoDeOtroCurso_Rechaza()
    {
        var resultado = await _controller.GuardarNotas(1, new NotasEvaluacionGuardarDto
            { Notas = new() { new() { IdAlumno = 99, Valor = 8 } } });

        resultado.Should().BeOfType<BadRequestObjectResult>();
        (await _context.NotasEvaluacion.CountAsync()).Should().Be(0);
    }
}
