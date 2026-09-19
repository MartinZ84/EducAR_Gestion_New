using EducAR.API.DTOs.Asistencia;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class AsistenciaServiceTests
{
    private readonly Mock<IAsistenciaRepository> _asistenciaRepoMock;
    private readonly Mock<ICursoRepository>      _cursoRepoMock;
    private readonly Mock<IDocenteRepository>    _docenteRepoMock;
    private readonly Mock<IDocenteMateriaCursoRepository> _asignacionRepoMock;
    private readonly Mock<IMatriculaRepository> _matriculaRepoMock;
    private readonly AsistenciaService           _service;

    public AsistenciaServiceTests()
    {
        _asistenciaRepoMock = new Mock<IAsistenciaRepository>();
        _cursoRepoMock      = new Mock<ICursoRepository>();
        _docenteRepoMock    = new Mock<IDocenteRepository>();
        _asignacionRepoMock = new Mock<IDocenteMateriaCursoRepository>();
        _matriculaRepoMock = new Mock<IMatriculaRepository>();

        _asignacionRepoMock.Setup(r => r.ObtenerPorDocente(It.IsAny<int>()))
            .ReturnsAsync(new List<DocenteMateriaCurso> { new() { IdCurso = 1 } });
        _matriculaRepoMock.Setup(r => r.ObtenerPorCurso(1, 1))
            .ReturnsAsync(new List<Matricula> { new() { IdAlumno = 1 }, new() { IdAlumno = 2 } });

        _service = new AsistenciaService(
            _asistenciaRepoMock.Object,
            _cursoRepoMock.Object,
            _docenteRepoMock.Object,
            _asignacionRepoMock.Object,
            _matriculaRepoMock.Object);
    }

    [Fact]
    public async Task Registrar_CursoNoExiste_RetornaError()
    {
        // Arrange
        _cursoRepoMock.Setup(r => r.ObtenerPorId(99, 1)).ReturnsAsync((Curso?)null);

        var dto = new AsistenciaRegistrarDto
        {
            IdCurso = 99,
            Fecha   = DateTime.Today,
            Alumnos = new List<AsistenciaAlumnoDto>
            {
                new() { IdAlumno = 1, Presente = true }
            }
        };

        // Act
        var (exito, mensaje) = await _service.Registrar(dto, 1, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Be("El curso no existe.");
    }

    [Fact]
    public async Task Registrar_CursoInactivo_RetornaError()
    {
        // Arrange
        var curso = TestDataBuilder.BuildCurso();
        curso.Activo = false;
        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(curso);

        var docente = TestDataBuilder.BuildDocente();
        _docenteRepoMock.Setup(r => r.ObtenerTodos(1)).ReturnsAsync(new List<Docente> { docente });

        var dto = new AsistenciaRegistrarDto
        {
            IdCurso = 1,
            Fecha   = DateTime.Today,
            Alumnos = new List<AsistenciaAlumnoDto>
            {
                new() { IdAlumno = 1, Presente = true }
            }
        };

        // Act
        var (exito, mensaje) = await _service.Registrar(dto, 1, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Be("El curso no está activo.");
    }

    [Fact]
    public async Task Registrar_SinAlumnos_RetornaError()
    {
        // Arrange
        var curso   = TestDataBuilder.BuildCurso();
        var docente = TestDataBuilder.BuildDocente();

        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(curso);
        _docenteRepoMock.Setup(r => r.ObtenerTodos(1)).ReturnsAsync(new List<Docente> { docente });

        var dto = new AsistenciaRegistrarDto
        {
            IdCurso = 1,
            Fecha   = DateTime.Today,
            Alumnos = new List<AsistenciaAlumnoDto>() // vacío
        };

        // Act
        var (exito, mensaje) = await _service.Registrar(dto, docente.IdUsuario, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Be("Debe incluir al menos un alumno.");
    }

    [Fact]
    public async Task Registrar_DocenteSinAsignacion_NoRegistra()
    {
        var docente = TestDataBuilder.BuildDocente();
        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(TestDataBuilder.BuildCurso());
        _docenteRepoMock.Setup(r => r.ObtenerTodos(1)).ReturnsAsync(new List<Docente> { docente });
        _asignacionRepoMock.Setup(r => r.ObtenerPorDocente(docente.IdDocente))
            .ReturnsAsync(new List<DocenteMateriaCurso>());

        var resultado = await _service.Registrar(new AsistenciaRegistrarDto
        {
            IdCurso = 1,
            Fecha = DateTime.Today,
            Alumnos = new() { new() { IdAlumno = 1, Presente = true } }
        }, docente.IdUsuario, 1);

        resultado.exito.Should().BeFalse();
        _asistenciaRepoMock.Verify(r => r.RegistrarLote(It.IsAny<List<Asistencia>>()), Times.Never);
    }

    [Fact]
    public async Task Registrar_AlumnoAjenoAlCurso_NoRegistra()
    {
        var docente = TestDataBuilder.BuildDocente();
        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(TestDataBuilder.BuildCurso());
        _docenteRepoMock.Setup(r => r.ObtenerTodos(1)).ReturnsAsync(new List<Docente> { docente });

        var resultado = await _service.Registrar(new AsistenciaRegistrarDto
        {
            IdCurso = 1,
            Fecha = DateTime.Today,
            Alumnos = new() { new() { IdAlumno = 1, Presente = true }, new() { IdAlumno = 99, Presente = false } }
        }, docente.IdUsuario, 1);

        resultado.exito.Should().BeFalse();
        _asistenciaRepoMock.Verify(r => r.RegistrarLote(It.IsAny<List<Asistencia>>()), Times.Never);
    }

    [Fact]
    public async Task Registrar_AsistenciaNueva_RegistraCorrectamente()
    {
        // Arrange
        var curso   = TestDataBuilder.BuildCurso();
        var docente = TestDataBuilder.BuildDocente();

        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(curso);
        _docenteRepoMock.Setup(r => r.ObtenerTodos(1)).ReturnsAsync(new List<Docente> { docente });
        _asistenciaRepoMock.Setup(r => r.ObtenerPorCursoYFecha(1, DateTime.Today)).ReturnsAsync(new List<Asistencia>());
        _asistenciaRepoMock.Setup(r => r.RegistrarLote(It.IsAny<List<Asistencia>>())).Returns(Task.CompletedTask);

        var dto = new AsistenciaRegistrarDto
        {
            IdCurso = 1,
            Fecha   = DateTime.Today,
            Alumnos = new List<AsistenciaAlumnoDto>
            {
                new() { IdAlumno = 1, Presente = true },
                new() { IdAlumno = 2, Presente = false }
            }
        };

        // Act
        var (exito, mensaje) = await _service.Registrar(dto, docente.IdUsuario, 1);

        // Assert
        exito.Should().BeTrue();
        mensaje.Should().Be("Asistencia registrada correctamente.");
        _asistenciaRepoMock.Verify(r => r.RegistrarLote(It.IsAny<List<Asistencia>>()), Times.Once);
    }

    [Fact]
    public async Task Registrar_AsistenciaExistente_ActualizaCorrectamente()
    {
        // Arrange
        var curso   = TestDataBuilder.BuildCurso();
        var docente = TestDataBuilder.BuildDocente();
        var asistenciasExistentes = new List<Asistencia>
        {
            new() { IdAsistencia = 1, IdAlumno = 1, IdCurso = 1, Fecha = DateTime.Today, Presente = false, Alumno = TestDataBuilder.BuildAlumno() }
        };

        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(curso);
        _docenteRepoMock.Setup(r => r.ObtenerTodos(1)).ReturnsAsync(new List<Docente> { docente });
        _asistenciaRepoMock.Setup(r => r.ObtenerPorCursoYFecha(1, DateTime.Today)).ReturnsAsync(asistenciasExistentes);
        _asistenciaRepoMock.Setup(r => r.ActualizarLote(It.IsAny<List<Asistencia>>())).Returns(Task.CompletedTask);

        var dto = new AsistenciaRegistrarDto
        {
            IdCurso = 1,
            Fecha   = DateTime.Today,
            Alumnos = new List<AsistenciaAlumnoDto>
            {
                new() { IdAlumno = 1, Presente = true },
                new() { IdAlumno = 2, Presente = false }
            }
        };

        // Act
        var (exito, mensaje) = await _service.Registrar(dto, docente.IdUsuario, 1);

        // Assert
        exito.Should().BeTrue();
        mensaje.Should().Be("Asistencia actualizada correctamente.");
        _asistenciaRepoMock.Verify(r => r.ActualizarLote(It.IsAny<List<Asistencia>>()), Times.Once);
        _asistenciaRepoMock.Verify(r => r.RegistrarLote(It.Is<List<Asistencia>>(items =>
            items.Count == 1 && items[0].IdAlumno == 2)), Times.Once);
    }

    [Fact]
    public async Task ObtenerPorCursoYFecha_CursoNoExiste_RetornaNull()
    {
        // Arrange
        _cursoRepoMock.Setup(r => r.ObtenerPorId(99, 1)).ReturnsAsync((Curso?)null);

        // Act
        var resultado = await _service.ObtenerPorCursoYFecha(99, DateTime.Today, 1);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task ObtenerPorCursoYFecha_CursoExiste_RetornaResumen()
    {
        // Arrange
        var curso = TestDataBuilder.BuildCurso();
        var asistencias = new List<Asistencia>
        {
            new() { IdAsistencia = 1, IdAlumno = 1, Presente = true,  Alumno = TestDataBuilder.BuildAlumno(1) },
            new() { IdAsistencia = 2, IdAlumno = 2, Presente = false, Alumno = TestDataBuilder.BuildAlumno(2) }
        };

        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(curso);
        _asistenciaRepoMock.Setup(r => r.ObtenerPorCursoYFecha(1, DateTime.Today)).ReturnsAsync(asistencias);

        // Act
        var resultado = await _service.ObtenerPorCursoYFecha(1, DateTime.Today, 1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.TotalAlumnos.Should().Be(2);
        resultado.Presentes.Should().Be(1);
        resultado.Ausentes.Should().Be(1);
    }
}
