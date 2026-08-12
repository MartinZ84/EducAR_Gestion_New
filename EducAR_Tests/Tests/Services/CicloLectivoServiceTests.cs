using EducAR.API.DTOs.CiclosLectivos;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class CicloLectivoServiceTests
{
    private readonly Mock<ICicloLectivoRepository> _repoMock;
    private readonly CicloLectivoService _service;

    public CicloLectivoServiceTests()
    {
        _repoMock = new Mock<ICicloLectivoRepository>();
        _service  = new CicloLectivoService(_repoMock.Object);
    }

    [Fact]
    public async Task Crear_AnioNuevo_RetornaExito()
    {
        // Arrange
        var dto = new CicloLectivoCreateDto
        {
            Anio        = 2027,
            FechaInicio = new DateTime(2027, 3, 1),
            FechaFin    = new DateTime(2027, 12, 15)
        };

        _repoMock.Setup(r => r.ExisteAnio(2027, 1, null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.Crear(It.IsAny<CicloLectivo>()))
                 .ReturnsAsync((CicloLectivo c) => { c.IdCicloLectivo = 1; return c; });

        // Act
        var (exito, mensaje, ciclo) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeTrue();
        ciclo.Should().NotBeNull();
        ciclo!.Anio.Should().Be(2027);
        mensaje.Should().Be("Ciclo lectivo creado correctamente.");
    }

    [Fact]
    public async Task Crear_AnioExistente_RetornaError()
    {
        // Arrange
        var dto = new CicloLectivoCreateDto
        {
            Anio        = 2026,
            FechaInicio = new DateTime(2026, 3, 1),
            FechaFin    = new DateTime(2026, 12, 15)
        };

        _repoMock.Setup(r => r.ExisteAnio(2026, 1, null)).ReturnsAsync(true);

        // Act
        var (exito, mensaje, ciclo) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        ciclo.Should().BeNull();
        mensaje.Should().Contain("Ya existe un ciclo lectivo");
    }

    [Fact]
    public async Task Crear_FechaFinMenorQueInicio_RetornaError()
    {
        // Arrange
        var dto = new CicloLectivoCreateDto
        {
            Anio        = 2027,
            FechaInicio = new DateTime(2027, 12, 1),
            FechaFin    = new DateTime(2027, 3, 1)  // fin antes que inicio
        };

        _repoMock.Setup(r => r.ExisteAnio(2027, 1, null)).ReturnsAsync(false);

        // Act
        var (exito, mensaje, ciclo) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        ciclo.Should().BeNull();
        mensaje.Should().Contain("fecha de fin debe ser posterior");
    }

    [Fact]
    public async Task Actualizar_CicloNoExiste_RetornaError()
    {
        // Arrange
        _repoMock.Setup(r => r.ObtenerPorId(99, 1)).ReturnsAsync((CicloLectivo?)null);

        var dto = new CicloLectivoUpdateDto
        {
            Anio        = 2027,
            FechaInicio = new DateTime(2027, 3, 1),
            FechaFin    = new DateTime(2027, 12, 15),
            Activo      = true
        };

        // Act
        var (exito, mensaje) = await _service.Actualizar(99, 1, dto);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Be("Ciclo lectivo no encontrado.");
    }

    [Fact]
    public async Task ObtenerTodos_RetornaListaOrdenada()
    {
        // Arrange
        var ciclos = new List<CicloLectivo>
        {
            TestDataBuilder.BuildCicloLectivo(1),
            TestDataBuilder.BuildCicloLectivo(2)
        };
        ciclos[1].Anio = 2025;

        _repoMock.Setup(r => r.ObtenerTodos(1)).ReturnsAsync(ciclos);

        // Act
        var resultado = await _service.ObtenerTodos(1);

        // Assert
        resultado.Should().HaveCount(2);
        resultado.Should().AllSatisfy(c => c.IdEscuela.Should().Be(1));
    }

    [Fact]
    public async Task ObtenerPorId_ExisteElCiclo_RetornaDto()
    {
        // Arrange
        var ciclo = TestDataBuilder.BuildCicloLectivo();
        _repoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(ciclo);

        // Act
        var resultado = await _service.ObtenerPorId(1, 1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.IdCicloLectivo.Should().Be(1);
        resultado.Anio.Should().Be(2026);
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNull()
    {
        // Arrange
        _repoMock.Setup(r => r.ObtenerPorId(99, 1)).ReturnsAsync((CicloLectivo?)null);

        // Act
        var resultado = await _service.ObtenerPorId(99, 1);

        // Assert
        resultado.Should().BeNull();
    }
}
