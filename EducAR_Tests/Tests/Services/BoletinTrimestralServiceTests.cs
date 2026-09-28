using EducAR.API.Data;
using EducAR.API.DTOs.Boletines;
using EducAR.API.Models;
using EducAR.API.Repositories;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class BoletinTrimestralServiceTests
{
    private readonly AppDbContext _context;
    private readonly BoletinService _service;
    private readonly Evaluacion _evaluacionA;
    private readonly Evaluacion _evaluacionB;

    public BoletinTrimestralServiceTests()
    {
        _context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);

        var escuela = new Escuela { IdEscuela = 1, Nombre = "Escuela", Direccion = "Calle 1" };
        var ciclo = new CicloLectivo
        {
            IdCicloLectivo = 1, IdEscuela = 1, Anio = 2026,
            FechaInicio = new(2026, 1, 1), FechaFin = new(2026, 12, 31), Escuela = escuela
        };
        var curso = new Curso
        {
            IdCurso = 10, IdEscuela = 1, IdCicloLectivo = 1, Grado = 4, Division = "A",
            Activo = true, Escuela = escuela, CicloLectivo = ciclo
        };
        var materiaMatematica = new Materia { IdMateria = 20, IdEscuela = 1, Nombre = "Matemática", Activo = true, Escuela = escuela };
        var materiaLengua = new Materia { IdMateria = 21, IdEscuela = 1, Nombre = "Lengua", Activo = true, Escuela = escuela };
        var periodoUno = new PeriodoEvaluacion
        {
            IdPeriodoEvaluacion = 30, IdCicloLectivo = 1, Nombre = "Primer trimestre",
            FechaInicio = new(2026, 3, 1), FechaFin = new(2026, 5, 31), CicloLectivo = ciclo
        };
        var periodoDos = new PeriodoEvaluacion
        {
            IdPeriodoEvaluacion = 31, IdCicloLectivo = 1, Nombre = "Segundo trimestre",
            FechaInicio = new(2026, 6, 1), FechaFin = new(2026, 8, 31), CicloLectivo = ciclo
        };
        var rol = new Rol { IdRol = 2, Nombre = "Docente" };
        var usuario = new Usuario
        {
            IdUsuario = 40, IdRol = 2, IdEscuela = 1, Nombre = "Ana", Apellido = "Docente",
            Email = "ana@example.invalid", NombreUsuario = "ana", HashContrasena = "hash",
            Escuela = escuela, Rol = rol
        };
        var docente = new Docente { IdDocente = 50, IdUsuario = 40, Usuario = usuario };
        var alumno = new Alumno
        {
            IdAlumno = 60, IdEscuela = 1, Dni = 12345678, Nombre = "Luis", Apellido = "Prueba",
            Activo = true, Escuela = escuela
        };
        _evaluacionA = new Evaluacion
        {
            IdEvaluacion = 70, IdCurso = 10, IdMateria = 20, IdPeriodoEvaluacion = 30,
            Titulo = "Prueba 1", Fecha = new(2026, 4, 1), Curso = curso,
            Materia = materiaMatematica, PeriodoEvaluacion = periodoUno
        };
        _evaluacionB = new Evaluacion
        {
            IdEvaluacion = 71, IdCurso = 10, IdMateria = 20, IdPeriodoEvaluacion = 30,
            Titulo = "Prueba 2", Fecha = new(2026, 5, 1), Curso = curso,
            Materia = materiaMatematica, PeriodoEvaluacion = periodoUno
        };
        var evaluacionOtroTrimestre = new Evaluacion
        {
            IdEvaluacion = 72, IdCurso = 10, IdMateria = 20, IdPeriodoEvaluacion = 31,
            Titulo = "Prueba trimestre 2", Fecha = new(2026, 6, 15), Curso = curso,
            Materia = materiaMatematica, PeriodoEvaluacion = periodoDos
        };
        var evaluacionPendiente = new Evaluacion
        {
            IdEvaluacion = 73, IdCurso = 10, IdMateria = 21, IdPeriodoEvaluacion = 30,
            Titulo = "Trabajo pendiente", Fecha = new(2026, 5, 15), Curso = curso,
            Materia = materiaLengua, PeriodoEvaluacion = periodoUno
        };

        _context.AddRange(escuela, ciclo, curso, materiaMatematica, materiaLengua,
            periodoUno, periodoDos, rol, usuario, docente, alumno);
        _context.DocenteMateriaCursos.AddRange(
            new DocenteMateriaCurso { IdDocenteMateriaCurso = 1, IdDocente = 50, IdCurso = 10, IdMateria = 20, Docente = docente, Curso = curso, Materia = materiaMatematica },
            new DocenteMateriaCurso { IdDocenteMateriaCurso = 2, IdDocente = 50, IdCurso = 10, IdMateria = 21, Docente = docente, Curso = curso, Materia = materiaLengua });
        _context.Matriculas.Add(new Matricula
        {
            IdMatricula = 80, IdEscuela = 1, IdAlumno = 60, IdCurso = 10, IdCicloLectivo = 1,
            Estado = EstadoMatricula.Activa, Escuela = escuela, Alumno = alumno, Curso = curso, CicloLectivo = ciclo
        });
        _context.Evaluaciones.AddRange(_evaluacionA, _evaluacionB, evaluacionOtroTrimestre, evaluacionPendiente);
        _context.NotasEvaluacion.AddRange(
            new NotaEvaluacion { IdNotaEvaluacion = 90, IdEvaluacion = 70, IdAlumno = 60, Valor = 8m, Evaluacion = _evaluacionA, Alumno = alumno },
            new NotaEvaluacion { IdNotaEvaluacion = 91, IdEvaluacion = 71, IdAlumno = 60, Valor = 8.01m, Evaluacion = _evaluacionB, Alumno = alumno },
            new NotaEvaluacion { IdNotaEvaluacion = 92, IdEvaluacion = 72, IdAlumno = 60, Valor = 2m, Evaluacion = evaluacionOtroTrimestre, Alumno = alumno });
        _context.Calificaciones.Add(new Calificacion
        {
            IdCalificacion = 100, IdAlumno = 60, IdMateria = 20, IdPeriodoEvaluacion = 30,
            ValorCalificacion = 1m, Activo = true, Alumno = alumno,
            Materia = materiaMatematica, PeriodoEvaluacion = periodoUno
        });
        _context.SaveChanges();

        var cursoRepository = new Mock<ICursoRepository>();
        cursoRepository.Setup(r => r.ObtenerPorId(10, 1)).ReturnsAsync(curso);
        var periodoRepository = new Mock<IPeriodoEvaluacionRepository>();
        periodoRepository.Setup(r => r.ObtenerPorId(30, 1)).ReturnsAsync(periodoUno);
        _service = new BoletinService(
            new BoletinRepository(_context), cursoRepository.Object, periodoRepository.Object, _context);
    }

    [Fact]
    public async Task Consulta_CalculaPromedioPorMateriaYNoConviertePendientesEnCero()
    {
        var boletin = (await _service.ObtenerPorCursoYPeriodo(10, 30, 1)).Single();

        boletin.Detalle.Should().HaveCount(2);
        boletin.Detalle.Single(d => d.IdMateria == 20).CalificacionFinal.Should().Be(8.01m);
        boletin.Detalle.Single(d => d.IdMateria == 21).CalificacionFinal.Should().BeNull();
        boletin.Detalle.Single(d => d.IdMateria == 21).ConceptoFinal.Should().BeNull();
        boletin.PromedioGeneral.Should().Be(8.01m);
        boletin.AnioLectivo.Should().Be(2026);
        boletin.RequiereRegeneracion.Should().BeTrue();
    }

    [Fact]
    public async Task GenerarYRegenerar_ActualizaMismoBoletin_YConservaObservacion()
    {
        var primeraGeneracion = await _service.GenerarParaAlumno(60, 10, 30, 1);
        primeraGeneracion.exito.Should().BeTrue();
        var idBoletin = primeraGeneracion.boletin!.IdBoletin;

        var guardado = await _context.Boletines.SingleAsync();
        guardado.ObservacionGeneral = "Observación del docente";
        await _context.SaveChangesAsync();
        _evaluacionB.Notas.Single(n => n.IdAlumno == 60).Valor = 7.5m;
        await _context.SaveChangesAsync();

        var regenerado = await _service.GenerarParaAlumno(60, 10, 30, 1);
        var boletinActualizado = regenerado.boletin!;

        boletinActualizado.IdBoletin.Should().Be(idBoletin);
        boletinActualizado.Detalle.Single(d => d.IdMateria == 20).CalificacionFinal.Should().Be(7.75m);
        boletinActualizado.Detalle.Single(d => d.IdMateria == 21).CalificacionFinal.Should().BeNull();
        boletinActualizado.ObservacionGeneral.Should().Be("Observación del docente");
        boletinActualizado.RequiereRegeneracion.Should().BeFalse();
        (await _context.Boletines.CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task ActualizarObservacion_ConservaDetallesDelBoletin()
    {
        var generado = await _service.GenerarParaAlumno(60, 10, 30, 1);

        var resultado = await _service.ActualizarObservacion(
            generado.boletin!.IdBoletin,
            new BoletinObservacionDto { ObservacionGeneral = "Seguimiento" },
            1);

        resultado.exito.Should().BeTrue();
        (await _context.DetallesBoletines.CountAsync()).Should().Be(2);
        (await _context.Boletines.SingleAsync()).ObservacionGeneral.Should().Be("Seguimiento");
    }

    [Theory]
    [InlineData(6.99, "Insuficiente")]
    [InlineData(7, "Bueno")]
    [InlineData(7.99, "Bueno")]
    [InlineData(8, "Muy bueno")]
    [InlineData(8.99, "Muy bueno")]
    [InlineData(9, "Excelente")]
    [InlineData(9.49, "Excelente")]
    [InlineData(9.5, "Sobresaliente")]
    [InlineData(10, "Sobresaliente")]
    public async Task Concepto_RespetaLosRangosAcordados(decimal promedio, string esperado)
    {
        _evaluacionA.Notas.Single(n => n.IdAlumno == 60).Valor = promedio;
        _evaluacionB.Notas.Single(n => n.IdAlumno == 60).Valor = promedio;
        await _context.SaveChangesAsync();

        var boletin = (await _service.ObtenerPorCursoYPeriodo(10, 30, 1)).Single();

        boletin.Detalle.Single(d => d.IdMateria == 20).CalificacionFinal.Should().Be(promedio);
        boletin.Detalle.Single(d => d.IdMateria == 20).ConceptoFinal.Should().Be(esperado);
    }
}