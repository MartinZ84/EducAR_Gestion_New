using EducAR.API.DTOs.PeriodosEvaluacion;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class PeriodoEvaluacionServiceTests
{
    private readonly Mock<IPeriodoEvaluacionRepository> _periodoRepoMock;
    private readonly Mock<ICicloLectivoRepository> _cicloRepoMock;
    private readonly PeriodoEvaluacionService _service;

    public PeriodoEvaluacionServiceTests()
    {
        _periodoRepoMock = new Mock<IPeriodoEvaluacionRepository>();
        _cicloRepoMock   = new Mock<ICicloLectivoRepository>();
        _service         = new PeriodoEvaluacionService(_periodoRepoMock.Object, _cicloRepoMock.Object);
    }

    [Fact]
    public async Task Crear_PeriodoValido_RetornaExito()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();
        var periodo = TestDataBuilder.BuildPeriodo();

        var dto = new PeriodoEvaluacionCreateDto
        {
            IdCicloLectivo = 1,
            Nombre         = "2do Trimestre",
            FechaInicio    = new DateTime(2026, 6, 1),
            FechaFin       = new DateTime(2026, 9, 30)
        };

        _cicloRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);
        _periodoRepoMock.Setup(r => r.ExisteNombre("2do Trimestre", 1, null)).ReturnsAsync(false);
        _periodoRepoMock.Setup(r => r.Crear(It.IsAny<PeriodoEvaluacion>()))
                        .ReturnsAsync((PeriodoEvaluacion p) => { p.IdPeriodoEvaluacion = 2; return p; });
        _periodoRepoMock.Setup(r => r.ObtenerPorId(2, 1)).ReturnsAsync(periodo);

        // Act
        var (exito, mensaje, result) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeTrue();
        mensaje.Should().Be("Período de evaluación creado correctamente.");
    }

    [Fact]
    public async Task Crear_FechaFueraDelCiclo_RetornaError()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();

        var dto = new PeriodoEvaluacionCreateDto
        {
            IdCicloLectivo = 1,
            Nombre         = "Trimestre Fuera",
            FechaInicio    = new DateTime(2027, 1, 1), // fuera del ciclo 2026
            FechaFin       = new DateTime(2027, 3, 31)
        };

        _cicloRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);
        _periodoRepoMock.Setup(r => r.ExisteNombre(It.IsAny<string>(), 1, null)).ReturnsAsync(false);

        // Act
        var (exito, mensaje, result) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("dentro del ciclo lectivo");
    }

    [Fact]
    public async Task Crear_FechaFinMenorQueInicio_RetornaError()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();
        var dto = new PeriodoEvaluacionCreateDto
        {
            IdCicloLectivo = 1,
            Nombre         = "Trimestre Invertido",
            FechaInicio    = new DateTime(2026, 6, 1),
            FechaFin       = new DateTime(2026, 4, 1) // fin antes de inicio
        };

        _cicloRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);
        _periodoRepoMock.Setup(r => r.ExisteNombre(It.IsAny<string>(), 1, null)).ReturnsAsync(false);

        // Act
        var (exito, mensaje, result) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("fecha de fin debe ser posterior");
    }

    [Fact]
    public async Task Crear_NombreDuplicado_RetornaError()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();
        var dto = new PeriodoEvaluacionCreateDto
        {
            IdCicloLectivo = 1,
            Nombre         = "1er Trimestre",
            FechaInicio    = new DateTime(2026, 6, 1),
            FechaFin       = new DateTime(2026, 8, 31)
        };

        _cicloRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);
        _periodoRepoMock.Setup(r => r.ExisteNombre("1er Trimestre", 1, null)).ReturnsAsync(true);

        // Act
        var (exito, mensaje, result) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("Ya existe un período");
    }

    [Fact]
    public async Task Eliminar_ConCalificaciones_RetornaError()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();
        var periodo = TestDataBuilder.BuildPeriodo();

        _cicloRepoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);
        _periodoRepoMock.Setup(r => r.Eliminar(1, 1))
                        .ReturnsAsync((false, "No se puede eliminar el período porque tiene calificaciones registradas."));

        // Act
        var (exito, mensaje) = await _service.Eliminar(1, 1, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("calificaciones registradas");
    }
}
