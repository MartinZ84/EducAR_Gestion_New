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

public class TutorConsultasControllerTests
{
    private readonly AppDbContext _context;
    private readonly TutorConsultasController _controller;

    public TutorConsultasControllerTests()
    {
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        _context.Escuelas.Add(new Escuela { IdEscuela = 1, Nombre = "Escuela", Direccion = "Calle 1" });
        _context.Roles.Add(new Rol { IdRol = 3, Nombre = "Tutor" });
        _context.Usuarios.Add(new Usuario
        {
            IdUsuario = 3, IdEscuela = 1, IdRol = 3, Nombre = "Tutor", Apellido = "Test",
            Email = "tutor@test.com", NombreUsuario = "tutor", HashContrasena = "hash"
        });
        _context.Tutores.Add(new Tutor { IdTutor = 1, IdUsuario = 3 });
        _context.Alumnos.AddRange(
            new Alumno { IdAlumno = 1, IdEscuela = 1, Nombre = "Vinculado", Apellido = "Test" },
            new Alumno { IdAlumno = 2, IdEscuela = 1, Nombre = "Ajeno", Apellido = "Test" });
        _context.AlumnoTutores.Add(new AlumnoTutor { IdAlumnoTutor = 1, IdTutor = 1, IdAlumno = 1, Activo = true });
        _context.CiclosLectivos.Add(new CicloLectivo { IdCicloLectivo = 1, IdEscuela = 1, Anio = DateTime.Now.Year });
        _context.Cursos.Add(new Curso { IdCurso = 1, IdEscuela = 1, IdCicloLectivo = 1, Division = "A" });
        _context.Materias.Add(new Materia { IdMateria = 1, IdEscuela = 1, Nombre = "Materia" });
        _context.PeriodosEvaluacion.Add(new PeriodoEvaluacion
            { IdPeriodoEvaluacion = 1, IdCicloLectivo = 1, Nombre = "Período" });
        _context.Asistencias.Add(new Asistencia
            { IdAsistencia = 1, IdAlumno = 2, IdCurso = 1, IdDocente = 1, Fecha = DateTime.Today });
        _context.Calificaciones.Add(new Calificacion
            { IdCalificacion = 1, IdAlumno = 2, IdMateria = 1, IdPeriodoEvaluacion = 1, ValorCalificacion = 8 });
        _context.SaveChanges();

        _controller = new TutorConsultasController(_context)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, "3"), new Claim("IdEscuela", "1")
                    }, "Test"))
                }
            }
        };
    }

    [Fact]
    public async Task MisAlumnos_IncluyeTutorNoPrincipal()
    {
        var resultado = await _controller.ObtenerMisAlumnos();
        resultado.Should().BeOfType<OkObjectResult>();
        var datos = ((OkObjectResult)resultado).Value as System.Collections.IEnumerable;
        datos!.Cast<object>().Should().ContainSingle();
    }

    [Fact]
    public async Task Consultas_AlumnoNoVinculado_NoExponenDatos()
    {
        (await _controller.ObtenerAsistencias(2)).Should().BeOfType<NotFoundResult>();
        (await _controller.ObtenerCalificaciones(2)).Should().BeOfType<NotFoundResult>();
        (await _controller.ObtenerNotas(2)).Should().BeOfType<NotFoundResult>();
    }
}
