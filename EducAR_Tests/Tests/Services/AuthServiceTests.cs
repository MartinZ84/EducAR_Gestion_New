using EducAR.API.DTOs.Auth;
using EducAR.API.Helpers;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IAuthRepository> _authRepoMock;
    private readonly JwtHelper _jwtHelper;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authRepoMock = new Mock<IAuthRepository>();

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]             = "ClaveSecretaMuyLargaParaTestingDeJWT32chars!!",
                ["Jwt:Issuer"]          = "EducAR.API",
                ["Jwt:Audience"]        = "EducAR.App",
                ["Jwt:ExpirationHours"] = "8"
            })
            .Build();

        _jwtHelper   = new JwtHelper(config);
        _authService = new AuthService(_authRepoMock.Object, _jwtHelper);
    }

    [Fact]
    public async Task Login_CredencialesValidas_RetornaToken()
    {
        // Arrange
        var usuario = TestDataBuilder.BuildUsuario();
        usuario.HashContrasena = BCrypt.Net.BCrypt.HashPassword("Admin123");

        _authRepoMock
            .Setup(r => r.ObtenerPorUsuarioYEscuela("admin", 1))
            .ReturnsAsync(usuario);

        var request = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena    = "Admin123",
            IdEscuela     = 1
        };

        // Act
        var resultado = await _authService.Login(request);

        // Assert
        resultado.Should().NotBeNull();
        resultado!.Token.Should().NotBeNullOrEmpty();
        resultado.NombreUsuario.Should().Be("admin");
        resultado.Rol.Should().Be("Administrador");
        resultado.IdEscuela.Should().Be(1);
    }

    [Fact]
    public async Task Login_ContrasenaIncorrecta_RetornaNull()
    {
        // Arrange
        var usuario = TestDataBuilder.BuildUsuario();
        usuario.HashContrasena = BCrypt.Net.BCrypt.HashPassword("Admin123");

        _authRepoMock
            .Setup(r => r.ObtenerPorUsuarioYEscuela("admin", 1))
            .ReturnsAsync(usuario);

        var request = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena    = "ContrasenaIncorrecta",
            IdEscuela     = 1
        };

        // Act
        var resultado = await _authService.Login(request);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Login_UsuarioNoExiste_RetornaNull()
    {
        // Arrange
        _authRepoMock
            .Setup(r => r.ObtenerPorUsuarioYEscuela("noexiste", 1))
            .ReturnsAsync((API.Models.Usuario?)null);

        var request = new LoginRequestDto
        {
            NombreUsuario = "noexiste",
            Contrasena    = "Admin123",
            IdEscuela     = 1
        };

        // Act
        var resultado = await _authService.Login(request);

        // Assert
        resultado.Should().BeNull();
    }

    [Fact]
    public async Task Login_Exitoso_ActualizaUltimoAcceso()
    {
        // Arrange
        var usuario = TestDataBuilder.BuildUsuario();
        usuario.HashContrasena = BCrypt.Net.BCrypt.HashPassword("Admin123");

        _authRepoMock
            .Setup(r => r.ObtenerPorUsuarioYEscuela("admin", 1))
            .ReturnsAsync(usuario);

        var request = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena    = "Admin123",
            IdEscuela     = 1
        };

        // Act
        await _authService.Login(request);

        // Assert
        _authRepoMock.Verify(r => r.ActualizarUltimoAcceso(usuario.IdUsuario), Times.Once);
    }
}
