using EducAR.API.DTOs.Escuelas;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class EscuelaServiceTests
{
    private readonly Mock<IEscuelaRepository> _repoMock;
    private readonly EscuelaService _service;

    public EscuelaServiceTests()
    {
        _repoMock = new Mock<IEscuelaRepository>();
        _service  = new EscuelaService(_repoMock.Object);
    }

    [Fact]
    public async Task Crear_NombreNuevo_RetornaExito()
    {
        // Arrange
        var dto = new EscuelaCreateDto
        {
            Nombre    = "Escuela Nueva",
            Direccion = "Av. Nueva 100"
        };

        _repoMock.Setup(r => r.ExisteNombre("Escuela Nueva", null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.Crear(It.IsAny<Escuela>()))
                 .ReturnsAsync((Escuela e) => { e.IdEscuela = 1; return e; });

        // Act
        var (exito, mensaje, escuela) = await _service.Crear(dto);

        // Assert
        exito.Should().BeTrue();
        escuela.Should().NotBeNull();
        escuela!.Nombre.Should().Be("Escuela Nueva");
        mensaje.Should().Be("Escuela creada correctamente.");
    }

    [Fact]
    public async Task Crear_NombreDuplicado_RetornaError()
    {
        // Arrange
        var dto = new EscuelaCreateDto { Nombre = "Escuela Existente", Direccion = "Test" };
        _repoMock.Setup(r => r.ExisteNombre("Escuela Existente", null)).ReturnsAsync(true);

        // Act
        var (exito, mensaje, escuela) = await _service.Crear(dto);

        // Assert
        exito.Should().BeFalse();
        escuela.Should().BeNull();
        mensaje.Should().Contain("Ya existe una escuela");
    }

    [Fact]
    public async Task Actualizar_EscuelaExiste_RetornaExito()
    {
        // Arrange
        var escuela = TestDataBuilder.BuildEscuela();
        _repoMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(escuela);
        _repoMock.Setup(r => r.ExisteNombre("Nuevo Nombre", 1)).ReturnsAsync(false);
        _repoMock.Setup(r => r.Actualizar(It.IsAny<Escuela>())).ReturnsAsync(true);

        var dto = new EscuelaUpdateDto
        {
            Nombre    = "Nuevo Nombre",
            Direccion = "Nueva Dirección",
            Activo    = true
        };

        // Act
        var (exito, mensaje) = await _service.Actualizar(1, dto);

        // Assert
        exito.Should().BeTrue();
        mensaje.Should().Be("Escuela actualizada correctamente.");
    }

    [Fact]
    public async Task Actualizar_EscuelaNoExiste_RetornaError()
    {
        // Arrange
        _repoMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Escuela?)null);
        var dto = new EscuelaUpdateDto { Nombre = "Test", Direccion = "Test", Activo = true };

        // Act
        var (exito, mensaje) = await _service.Actualizar(99, dto);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Be("Escuela no encontrada.");
    }

    [Fact]
    public async Task ObtenerPorId_ExisteEscuela_RetornaDto()
    {
        // Arrange
        var escuela = TestDataBuilder.BuildEscuela();
        _repoMock.Setup(r => r.ObtenerPorId(1)).ReturnsAsync(escuela);

        // Act
        var resultado = await _service.ObtenerPorId(1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.IdEscuela.Should().Be(1);
        resultado.Nombre.Should().Be("Escuela Test");
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_RetornaNull()
    {
        // Arrange
        _repoMock.Setup(r => r.ObtenerPorId(99)).ReturnsAsync((Escuela?)null);

        // Act
        var resultado = await _service.ObtenerPorId(99);

        // Assert
        resultado.Should().BeNull();
    }
}
