using EducAR.API.Controllers;
using EducAR.API.DTOs.Escuelas;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class EscuelasControllerTests
{
    private readonly Mock<IEscuelaService> _serviceMock;
    private readonly EscuelasController _controller;

    public EscuelasControllerTests()
    {
        _serviceMock = new Mock<IEscuelaService>();
        _controller  = new EscuelasController(_serviceMock.Object);

        // Simular usuario autenticado
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Role, "Administrador"),
            new("IdEscuela", "1")
        };
        var identity  = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = principal }
        };
    }

    [Fact]
    public async Task ObtenerTodas_RetornaOkConResultadoPaginado()
    {
        // Arrange
        var filtro    = new FiltroNombreDto { Pagina = 1, Cantidad = 10 };
        var resultado = new ResultadoPaginadoDto<EscuelaResponseDto>
        {
            PaginaActual       = 1,
            TotalPaginas       = 1,
            TotalRegistros     = 2,
            RegistrosPorPagina = 10,
            Datos = new List<EscuelaResponseDto>
            {
                new() { IdEscuela = 1, Nombre = "Escuela 1", Activo = true },
                new() { IdEscuela = 2, Nombre = "Escuela 2", Activo = true }
            }
        };

        _serviceMock.Setup(s => s.ObtenerTodasPaginado(It.IsAny<FiltroNombreDto>()))
                    .ReturnsAsync(resultado);

        // Act
        var accion = await _controller.ObtenerTodas(filtro);

        // Assert
        var okResult = accion.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
        var datos = okResult.Value.Should().BeOfType<ResultadoPaginadoDto<EscuelaResponseDto>>().Subject;
        datos.TotalRegistros.Should().Be(2);
    }

    [Fact]
    public async Task ObtenerPorId_EscuelaExiste_Retorna200()
    {
        // Arrange
        var escuela = new EscuelaResponseDto { IdEscuela = 1, Nombre = "Escuela Test", Activo = true };
        _serviceMock.Setup(s => s.ObtenerPorId(1)).ReturnsAsync(escuela);

        // Act
        var accion = await _controller.ObtenerPorId(1);

        // Assert
        var okResult = accion.Should().BeOfType<OkObjectResult>().Subject;
        okResult.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_Retorna404()
    {
        // Arrange
        _serviceMock.Setup(s => s.ObtenerPorId(99)).ReturnsAsync((EscuelaResponseDto?)null);

        // Act
        var accion = await _controller.ObtenerPorId(99);

        // Assert
        accion.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Crear_DatosValidos_Retorna201()
    {
        // Arrange
        var dto = new EscuelaCreateDto { Nombre = "Nueva Escuela", Direccion = "Test 100" };
        var escuela = new EscuelaResponseDto { IdEscuela = 1, Nombre = "Nueva Escuela", Activo = true };

        _serviceMock.Setup(s => s.Crear(dto)).ReturnsAsync((true, "Escuela creada correctamente.", escuela));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        var createdResult = accion.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
    }

    [Fact]
    public async Task Crear_NombreDuplicado_Retorna400()
    {
        // Arrange
        var dto = new EscuelaCreateDto { Nombre = "Duplicada", Direccion = "Test" };
        _serviceMock.Setup(s => s.Crear(dto))
                    .ReturnsAsync((false, "Ya existe una escuela con ese nombre.", (EscuelaResponseDto?)null));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<BadRequestObjectResult>();
    }
}
