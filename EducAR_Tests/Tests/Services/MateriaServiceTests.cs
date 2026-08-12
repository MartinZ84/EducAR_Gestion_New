using EducAR.API.DTOs.Materias;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class MateriaServiceTests
{
    private readonly Mock<IMateriaRepository> _repoMock;
    private readonly MateriaService _service;

    public MateriaServiceTests()
    {
        _repoMock = new Mock<IMateriaRepository>();
        _service  = new MateriaService(_repoMock.Object);
    }

    [Fact]
    public async Task Crear_NombreNuevo_RetornaExito()
    {
        // Arrange
        var dto = new MateriaCreateDto { Nombre = "Física", Descripcion = "Ciencia exacta" };

        _repoMock.Setup(r => r.ExisteNombre("Física", 1, null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.Crear(It.IsAny<Materia>()))
                 .ReturnsAsync((Materia m) => { m.IdMateria = 1; return m; });

        // Act
        var (exito, mensaje, materia) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeTrue();
        materia.Should().NotBeNull();
        materia!.Nombre.Should().Be("Física");
    }

    [Fact]
    public async Task Crear_NombreDuplicado_RetornaError()
    {
        // Arrange
        var dto = new MateriaCreateDto { Nombre = "Matemática" };
        _repoMock.Setup(r => r.ExisteNombre("Matemática", 1, null)).ReturnsAsync(true);

        // Act
        var (exito, mensaje, materia) = await _service.Crear(dto, 1);

        // Assert
        exito.Should().BeFalse();
        materia.Should().BeNull();
        mensaje.Should().Contain("Ya existe una materia");
    }

    [Fact]
    public async Task Actualizar_MateriaExiste_RetornaExito()
    {
        // Arrange
        var materia = TestDataBuilder.BuildMateria();
        _repoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(materia);
        _repoMock.Setup(r => r.ExisteNombre("Nuevo Nombre", 1, 1)).ReturnsAsync(false);
        _repoMock.Setup(r => r.Actualizar(It.IsAny<Materia>())).ReturnsAsync(true);

        var dto = new MateriaUpdateDto { Nombre = "Nuevo Nombre", Activo = true };

        // Act
        var (exito, mensaje) = await _service.Actualizar(1, 1, dto);

        // Assert
        exito.Should().BeTrue();
        mensaje.Should().Be("Materia actualizada correctamente.");
    }

    [Fact]
    public async Task Actualizar_MateriaNoExiste_RetornaError()
    {
        // Arrange
        _repoMock.Setup(r => r.ObtenerPorId(99, 1)).ReturnsAsync((Materia?)null);
        var dto = new MateriaUpdateDto { Nombre = "Test", Activo = true };

        // Act
        var (exito, mensaje) = await _service.Actualizar(99, 1, dto);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Be("Materia no encontrada.");
    }

    [Fact]
    public async Task Eliminar_MateriaExiste_RetornaTrue()
    {
        // Arrange
        _repoMock.Setup(r => r.Eliminar(1, 1)).ReturnsAsync(true);

        // Act
        var resultado = await _service.Eliminar(1, 1);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task Eliminar_MateriaNoExiste_RetornaFalse()
    {
        // Arrange
        _repoMock.Setup(r => r.Eliminar(99, 1)).ReturnsAsync(false);

        // Act
        var resultado = await _service.Eliminar(99, 1);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ObtenerTodas_RetornaLista()
    {
        // Arrange
        var materias = new List<Materia>
        {
            TestDataBuilder.BuildMateria(1),
            TestDataBuilder.BuildMateria(2),
            TestDataBuilder.BuildMateria(3)
        };
        _repoMock.Setup(r => r.ObtenerTodas(1)).ReturnsAsync(materias);

        // Act
        var resultado = await _service.ObtenerTodas(1);

        // Assert
        resultado.Should().HaveCount(3);
    }
}
