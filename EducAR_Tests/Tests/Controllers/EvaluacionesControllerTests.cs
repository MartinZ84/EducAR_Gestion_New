using EducAR.API.Controllers;
using EducAR.API.DTOs.Evaluaciones;
using EducAR.API.Services;
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

        _controller = new EvaluacionesController(new EvaluacionService(_context))
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

    private static EvaluacionCrearDto Nueva() => new()
    {
        IdCurso = 1, IdMateria = 1, IdPeriodoEvaluacion = 1,
        Titulo = "Fracciones", Temario = "Suma y resta", Descripcion = "Resolución de problemas",
        Fecha = new DateTime(DateTime.Now.Year, 6, 1)
    };

    [Fact]
    public async Task Flujo_CrearCalificarLoteEditarConsultar()
    {
        _context.Alumnos.Add(new Alumno { IdAlumno = 2, IdEscuela = 1, Nombre = "Ana", Apellido = "Dos" });
        _context.Alumnos.Add(new Alumno { IdAlumno = 3, IdEscuela = 1, Nombre = "Luis", Apellido = "Tres" });
        _context.Matriculas.AddRange(
            new Matricula { IdAlumno = 2, IdEscuela = 1, IdCurso = 1, IdCicloLectivo = 1 },
            new Matricula { IdAlumno = 3, IdEscuela = 1, IdCurso = 1, IdCicloLectivo = 1 });
        await _context.SaveChangesAsync();
        (await _controller.Crear(Nueva())).Should().BeOfType<OkObjectResult>();
        var evaluacion = await _context.Evaluaciones.SingleAsync(e => e.Titulo == "Fracciones");
        var servicio = new EvaluacionService(_context);
        var acceso = new AccesoEvaluacion(2, 1, false);
        var grilla = await servicio.ObtenerAlumnos(evaluacion.IdEvaluacion, acceso);
        var json = System.Text.Json.JsonSerializer.SerializeToElement(grilla.Datos);
        json.GetArrayLength().Should().Be(3);
        json.EnumerateArray().Should().OnlyContain(a => a.GetProperty("Valor").ValueKind == System.Text.Json.JsonValueKind.Null);
        var guardado = await servicio.GuardarNotas(evaluacion.IdEvaluacion, new() { Notas = new()
            { new() { IdAlumno = 1, Valor = 7.5m }, new() { IdAlumno = 2, Valor = 9 } } }, acceso);
        guardado.Estado.Should().Be(200);
        ((ResultadoNotas)guardado.Datos!).Resultados.Should().OnlyContain(n => n.Accion == "registrada");
        (await _context.NotasEvaluacion.CountAsync(n => n.IdAlumno == 3)).Should().Be(0);
        var editado = await servicio.GuardarNotas(evaluacion.IdEvaluacion, new() { Notas = new()
            { new() { IdAlumno = 1, Valor = 8 } } }, acceso);
        ((ResultadoNotas)editado.Datos!).Resultados.Single().Accion.Should().Be("actualizada");
        (await _context.NotasEvaluacion.CountAsync()).Should().Be(2);
        var consulta = System.Text.Json.JsonSerializer.SerializeToElement((await servicio.ObtenerAlumnos(evaluacion.IdEvaluacion, acceso)).Datos);
        consulta.EnumerateArray().Single(a => a.GetProperty("IdAlumno").GetInt32() == 1).GetProperty("Valor").GetDecimal().Should().Be(8);
        consulta.EnumerateArray().Single(a => a.GetProperty("IdAlumno").GetInt32() == 2).GetProperty("Valor").GetDecimal().Should().Be(9);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(11)]
    [InlineData(5.555)]
    public async Task NotaInvalida_NoGuarda(decimal valor)
    {
        (await _controller.GuardarNotas(1, new() { Notas = new() { new() { IdAlumno = 1, Valor = valor } } }))
            .Should().BeOfType<BadRequestObjectResult>();
        (await _context.NotasEvaluacion.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task LoteConAlumnoAjeno_NoGuardaTampocoLaNotaValida()
    {
        var resultado = await _controller.GuardarNotas(1, new() { Notas = new()
            { new() { IdAlumno = 1, Valor = 8 }, new() { IdAlumno = 99, Valor = 7 } } });
        resultado.Should().BeOfType<BadRequestObjectResult>();
        (await _context.NotasEvaluacion.CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task MatriculaDeOtroAnio_Rechazada()
    {
        (await _context.Matriculas.SingleAsync()).IdCicloLectivo = 2;
        await _context.SaveChangesAsync();
        (await _controller.GuardarNotas(1, new() { Notas = new() { new() { IdAlumno = 1, Valor = 8 } } }))
            .Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Duplicados_Rechazados()
    {
        (await _controller.GuardarNotas(1, new() { Notas = new()
            { new() { IdAlumno = 1, Valor = 8 }, new() { IdAlumno = 1, Valor = 9 } } }))
            .Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Permisos_DocenteAjenoYAdminDeOtraEscuelaNoPuedenCrear()
    {
        var servicio = new EvaluacionService(_context);
        (await servicio.Crear(Nueva(), new(999, 1, false))).Estado.Should().Be(403);
        (await servicio.Crear(Nueva(), new(999, 2, true))).Estado.Should().Be(403);
        (await servicio.Crear(Nueva(), new(999, 1, true))).Estado.Should().Be(200);
        (await servicio.GuardarNotas(1, new() { Notas = new() { new() { IdAlumno = 1, Valor = 8 } } }, new(999, 1, false))).Estado.Should().Be(403);
        (await servicio.ObtenerAlumnos(1, new(999, 1, false))).Estado.Should().Be(403);
    }

    [Fact]
    public async Task CalificacionPrevia_SeConserva()
    {
        _context.Calificaciones.Add(new Calificacion { IdAlumno = 1, IdMateria = 1, IdPeriodoEvaluacion = 1, ValorCalificacion = 6, Observacion = "Previa" });
        await _context.SaveChangesAsync();
        await _controller.GuardarNotas(1, new() { Notas = new() { new() { IdAlumno = 1, Valor = 9 } } });
        var final = await _context.Calificaciones.SingleAsync();
        final.ValorCalificacion.Should().Be(6);
        final.Observacion.Should().Be("Previa");
    }

    [Fact]
    public async Task EditarYArchivar_ConservaNotasYNoPermiteArchivarCalificada()
    {
        var dto = Nueva();
        (await _controller.Editar(1, dto)).Should().BeOfType<OkObjectResult>();
        (await _context.Evaluaciones.FindAsync(1))!.Temario.Should().Be(dto.Temario);
        await _controller.GuardarNotas(1, new() { Notas = new() { new() { IdAlumno = 1, Valor = 8 } } });
        (await _controller.Archivar(1)).Should().BeOfType<BadRequestObjectResult>();
        (await _controller.Archivar(2)).Should().BeOfType<OkObjectResult>();
        (await _context.Evaluaciones.FindAsync(2))!.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task Crear_PeriodoAjenoOFechaFueraDeRango_Rechazado()
    {
        var dto = Nueva(); dto.IdPeriodoEvaluacion = 99;
        (await _controller.Crear(dto)).Should().BeOfType<BadRequestObjectResult>();
        dto.IdPeriodoEvaluacion = 1; dto.Fecha = new DateTime(DateTime.Now.Year, 1, 1);
        (await _controller.Crear(dto)).Should().BeOfType<BadRequestObjectResult>();
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
