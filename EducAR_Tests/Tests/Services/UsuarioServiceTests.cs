using EducAR.API.DTOs.Paginacion;
using EducAR.API.DTOs.Usuarios;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class UsuarioServiceTests
{
    private readonly Mock<IUsuarioRepository> _repoMock;
    private readonly UsuarioService _service;

    public UsuarioServiceTests()
    {
        _repoMock = new Mock<IUsuarioRepository>();
        _service  = new UsuarioService(_repoMock.Object);
    }

    [Fact]
    public async Task Crear_DatosValidos_RetornaExito()
    {
        // Arrange
        var dto = new UsuarioCreateDto
        {
            IdRol         = 1,
            IdEscuela     = 1,
            Dni           = 99999999,
            Nombre        = "Nuevo",
            Apellido      = "Usuario",
            Email         = "nuevo@test.com",
            NombreUsuario = "nuevousuario",
            Contrasena    = "Test123"
        };

        var usuarioCreado = TestDataBuilder.BuildUsuario();

        _repoMock.Setup(r => r.ExisteNombreUsuario("nuevousuario", 1)).ReturnsAsync(false);
        _repoMock.Setup(r => r.ExisteDni(99999999, 1, null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.ExisteEmail("nuevo@test.com", 1, null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.Crear(It.IsAny<Usuario>()))
                 .ReturnsAsync((Usuario u) => { u.IdUsuario = 1; return u; });
        _repoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(usuarioCreado);

        // Act
        var (exito, mensaje, usuario) = await _service.Crear(dto);

        // Assert
        exito.Should().BeTrue();
        usuario.Should().NotBeNull();
        mensaje.Should().Be("Usuario creado correctamente.");
    }

    [Fact]
    public async Task Crear_NombreUsuarioDuplicado_RetornaError()
    {
        // Arrange
        var dto = new UsuarioCreateDto
        {
            NombreUsuario = "admin",
            IdEscuela     = 1,
            Contrasena    = "Test123"
        };

        _repoMock.Setup(r => r.ExisteNombreUsuario("admin", 1)).ReturnsAsync(true);

        // Act
        var (exito, mensaje, usuario) = await _service.Crear(dto);

        // Assert
        exito.Should().BeFalse();
        usuario.Should().BeNull();
        mensaje.Should().Contain("nombre de usuario ya existe");
    }

    [Fact]
    public async Task Crear_DniDuplicado_RetornaError()
    {
        // Arrange
        var dto = new UsuarioCreateDto
        {
            NombreUsuario = "nuevousuario",
            IdEscuela     = 1,
            Dni           = 12345678,
            Contrasena    = "Test123"
        };

        _repoMock.Setup(r => r.ExisteNombreUsuario("nuevousuario", 1)).ReturnsAsync(false);
        _repoMock.Setup(r => r.ExisteDni(12345678, 1, null)).ReturnsAsync(true);

        // Act
        var (exito, mensaje, usuario) = await _service.Crear(dto);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("DNI");
    }

    [Fact]
    public async Task Crear_EmailDuplicado_RetornaError()
    {
        // Arrange
        var dto = new UsuarioCreateDto
        {
            NombreUsuario = "nuevousuario",
            IdEscuela     = 1,
            Dni           = 99999998,
            Email         = "existente@test.com",
            Contrasena    = "Test123"
        };

        _repoMock.Setup(r => r.ExisteNombreUsuario("nuevousuario", 1)).ReturnsAsync(false);
        _repoMock.Setup(r => r.ExisteDni(99999998, 1, null)).ReturnsAsync(false);
        _repoMock.Setup(r => r.ExisteEmail("existente@test.com", 1, null)).ReturnsAsync(true);

        // Act
        var (exito, mensaje, usuario) = await _service.Crear(dto);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("email");
    }

    [Fact]
    public async Task Actualizar_UsuarioNoExiste_RetornaError()
    {
        // Arrange
        _repoMock.Setup(r => r.ObtenerPorId(99, 1)).ReturnsAsync((Usuario?)null);

        var dto = new UsuarioUpdateDto
        {
            IdRol    = 1,
            Dni      = 12345678,
            Nombre   = "Test",
            Apellido = "Test",
            Email    = "test@test.com",
            Activo   = true
        };

        // Act
        var (exito, mensaje) = await _service.Actualizar(99, 1, dto);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Be("Usuario no encontrado.");
    }

    [Fact]
    public async Task Eliminar_UsuarioExiste_RetornaTrue()
    {
        // Arrange
        _repoMock.Setup(r => r.Eliminar(1, 1)).ReturnsAsync(true);

        // Act
        var resultado = await _service.Eliminar(1, 1);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task ObtenerPorId_UsuarioExiste_RetornaDto()
    {
        // Arrange
        var usuario = TestDataBuilder.BuildUsuario();
        _repoMock.Setup(r => r.ObtenerPorId(1, 1)).ReturnsAsync(usuario);

        // Act
        var resultado = await _service.ObtenerPorId(1, 1);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.IdUsuario.Should().Be(1);
        resultado.NombreUsuario.Should().Be("admin");
    }
}
