using EducAR.API.Controllers;
using EducAR.API.DTOs.Auth;
using EducAR.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _controller;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _controller      = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task Login_CredencialesValidas_Retorna200ConToken()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena    = "Admin123",
            IdEscuela     = 1
        };

        var response = new LoginResponseDto
        {
            Token          = "jwt.token.aqui",
            NombreUsuario  = "admin",
            NombreCompleto = "Admin Test",
            Rol            = "Administrador",
            IdEscuela      = 1,
            Expiracion     = DateTime.UtcNow.AddHours(8)
        };

        _authServiceMock.Setup(s => s.Login(request)).ReturnsAsync(response);

        // Act
        var resultado = await _controller.Login(request);

        // Assert
        var okResult = resultado.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        var dto = okResult.Value.Should().BeOfType<LoginResponseDto>().Subject;
        dto.Token.Should().Be("jwt.token.aqui");
        dto.Rol.Should().Be("Administrador");
    }

    [Fact]
    public async Task Login_CredencialesInvalidas_Retorna401()
    {
        // Arrange
        var request = new LoginRequestDto
        {
            NombreUsuario = "admin",
            Contrasena    = "Incorrecta",
            IdEscuela     = 1
        };

        _authServiceMock.Setup(s => s.Login(request)).ReturnsAsync((LoginResponseDto?)null);

        // Act
        var resultado = await _controller.Login(request);

        // Assert
        var unauthorizedResult = resultado.Should().BeOfType<UnauthorizedObjectResult>().Subject;
        unauthorizedResult.StatusCode.Should().Be(401);
    }
}
