using EducAR.API.DTOs.Cursos;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class CursoServiceTests
{
    private readonly Mock<ICursoRepository> _cursoRepoMock;
    private readonly Mock<ICicloLectivoRepository> _cicloRepoMock;
    private readonly CursoService _service;

    public CursoServiceTests()
    {
        _cursoRepoMock = new Mock<ICursoRepository>();
        _cicloRepoMock = new Mock<ICicloLectivoRepository>();
        _service       = new CursoService(_cursoRepoMock.Object, _cicloRepoMock.Object);
    }

    [Fact]
    public async Task Crear_CursoNuevo_RetornaExito()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();
        var dto = new CursoCreateDto
        {
            IdCicloLectivo = 1,
            Grado          = 2,
            Division       = "B",
            Turno          = "Tarde"
        };

        _cicloRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);
        _cursoRepoMock.Setup(r => r.ExisteCurso(2, "B", "Tarde", 1, null)).ReturnsAsync(false);
        _cursoRepoMock.Setup(r => r.Crear(It.IsAny<Curso>()))
                      .ReturnsAsync((Curso c) => { c.IdCurso = 1; return c; });
        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1))
                      .ReturnsAsync(TestDataBuilder.BuildCurso());

        // Act
        var (exito, mensaje, curso) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeTrue();
        curso.Should().NotBeNull();
        mensaje.Should().Be("Curso creado correctamente.");
    }

    [Fact]
    public async Task Crear_CicloInactivo_RetornaError()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();
        ciclo.Activo = false;

        _cicloRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);

        var dto = new CursoCreateDto { IdCicloLectivo = 1, Grado = 1, Division = "A" };

        // Act
        var (exito, mensaje, curso) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("ciclo lectivo inactivo");
    }

    [Fact]
    public async Task Crear_CursoDuplicado_RetornaError()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();
        _cicloRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);
        _cursoRepoMock.Setup(r => r.ExisteCurso(1, "A", "Mañana", 1, null)).ReturnsAsync(true);

        var dto = new CursoCreateDto
        {
            IdCicloLectivo = 1,
            Grado          = 1,
            Division       = "A",
            Turno          = "Mañana"
        };

        // Act
        var (exito, mensaje, curso) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        curso.Should().BeNull();
    }

    [Fact]
    public async Task Crear_CicloNoExiste_RetornaError()
    {
        // Arrange
        _cicloRepoMock.Setup(r => r.ObtenerPorId(99, 1)).ReturnsAsync((CicloLectivo?)null);
        var dto = new CursoCreateDto { IdCicloLectivo = 99, Grado = 1, Division = "A" };

        // Act
        var (exito, mensaje, curso) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Be("El ciclo lectivo no existe.");
    }

    [Fact]
    public async Task Eliminar_ConAlumnosInscriptos_RetornaError()
    {
        // Arrange
        var curso = TestDataBuilder.BuildCurso();
        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(curso);
        _cursoRepoMock.Setup(r => r.TieneAlumnosInscriptos(1)).ReturnsAsync(true);

        // Act
        var (exito, mensaje) = await _service.Eliminar(1, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("alumnos inscriptos");
    }

    [Fact]
    public async Task Eliminar_ConAsistencias_RetornaError()
    {
        // Arrange
        var curso = TestDataBuilder.BuildCurso();
        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(curso);
        _cursoRepoMock.Setup(r => r.TieneAlumnosInscriptos(1)).ReturnsAsync(false);
        _cursoRepoMock.Setup(r => r.TieneAsistencias(1)).ReturnsAsync(true);

        // Act
        var (exito, mensaje) = await _service.Eliminar(1, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("asistencias registradas");
    }

    [Fact]
    public async Task Eliminar_SinDependencias_RetornaExito()
    {
        // Arrange
        var curso = TestDataBuilder.BuildCurso();
        _cursoRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(curso);
        _cursoRepoMock.Setup(r => r.TieneAlumnosInscriptos(1)).ReturnsAsync(false);
        _cursoRepoMock.Setup(r => r.TieneAsistencias(1)).ReturnsAsync(false);
        _cursoRepoMock.Setup(r => r.TieneCalificaciones(1)).ReturnsAsync(false);
        _cursoRepoMock.Setup(r => r.Eliminar(1, 1)).ReturnsAsync(true);

        // Act
        var (exito, mensaje) = await _service.Eliminar(1, 1);

        // Assert
        exito.Should().BeTrue();
        mensaje.Should().Be("Curso dado de baja correctamente.");
    }
}
