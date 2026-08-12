using EducAR.API.Controllers;
using EducAR.API.DTOs.Docentes;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class DocentesControllerTests
{
    private readonly Mock<IDocenteService> _serviceMock;
    private readonly DocentesController _controller;

    public DocentesControllerTests()
    {
        _serviceMock = new Mock<IDocenteService>();
        _controller  = new DocentesController(_serviceMock.Object);

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, "1"),
            new(ClaimTypes.Role, "Administrador"),
            new("IdEscuela", "1")
        };
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(claims, "Test"))
            }
        };
    }

    [Fact]
    public async Task ObtenerTodos_RetornaOkConPaginacion()
    {
        // Arrange
        var filtro    = new FiltroPersonaDto { Pagina = 1, Cantidad = 10 };
        var resultado = new ResultadoPaginadoDto<DocenteResponseDto>
        {
            PaginaActual       = 1,
            TotalPaginas       = 1,
            TotalRegistros     = 2,
            RegistrosPorPagina = 10,
            Datos = new List<DocenteResponseDto>
            {
                new() { IdDocente = 1, Nombre = "Pedro", Apellido = "Pérez" },
                new() { IdDocente = 2, Nombre = "Laura", Apellido = "Gómez" }
            }
        };

        _serviceMock.Setup(s => s.ObtenerTodosPaginado(1, It.IsAny<FiltroPersonaDto>()))
                    .ReturnsAsync(resultado);

        // Act
        var accion = await _controller.ObtenerTodos(filtro);

        // Assert
        var okResult = accion.Should().BeOfType<OkObjectResult>().Subject;
        var datos    = okResult.Value.Should().BeOfType<ResultadoPaginadoDto<DocenteResponseDto>>().Subject;
        datos.TotalRegistros.Should().Be(2);
    }

    [Fact]
    public async Task Crear_DatosValidos_Retorna201()
    {
        // Arrange
        var dto = new DocenteCreateDto
        {
            Dni           = 30000001,
            Nombre        = "Laura",
            Apellido      = "Gómez",
            Email         = "laura@test.com",
            NombreUsuario = "lgomez",
            Contrasena    = "Docente123"
        };
        var docente = new DocenteResponseDto { IdDocente = 1, Nombre = "Laura", Apellido = "Gómez" };

        _serviceMock.Setup(s => s.Crear(dto, 1))
                    .ReturnsAsync((true, "Docente creado correctamente.", docente));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Crear_UsuarioDuplicado_Retorna400()
    {
        // Arrange
        var dto = new DocenteCreateDto
        {
            Dni           = 30000001,
            Nombre        = "Laura",
            Apellido      = "Gómez",
            Email         = "laura@test.com",
            NombreUsuario = "lgomez",
            Contrasena    = "Docente123"
        };

        _serviceMock.Setup(s => s.Crear(dto, 1))
                    .ReturnsAsync((false, "El nombre de usuario ya existe en esta escuela.", (DocenteResponseDto?)null));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Actualizar_DocenteExiste_Retorna200()
    {
        // Arrange
        var dto = new DocenteUpdateDto { Nombre = "Laura", Apellido = "Gómez", Email = "nueva@test.com", Activo = true };
        _serviceMock.Setup(s => s.Actualizar(1, 1, dto))
                    .ReturnsAsync((true, "Docente actualizado correctamente."));

        // Act
        var accion = await _controller.Actualizar(1, dto);

        // Assert
        accion.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Actualizar_NoExiste_Retorna404()
    {
        // Arrange
        var dto = new DocenteUpdateDto { Nombre = "Test", Apellido = "Test", Email = "test@test.com", Activo = true };
        _serviceMock.Setup(s => s.Actualizar(99, 1, dto))
                    .ReturnsAsync((false, "Docente no encontrado."));

        // Act
        var accion = await _controller.Actualizar(99, dto);

        // Assert
        accion.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Eliminar_DocenteExiste_Retorna200()
    {
        // Arrange
        _serviceMock.Setup(s => s.Eliminar(1, 1)).ReturnsAsync(true);

        // Act
        var accion = await _controller.Eliminar(1);

        // Assert
        accion.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Eliminar_NoExiste_Retorna404()
    {
        // Arrange
        _serviceMock.Setup(s => s.Eliminar(99, 1)).ReturnsAsync(false);

        // Act
        var accion = await _controller.Eliminar(99);

        // Assert
        accion.Should().BeOfType<NotFoundObjectResult>();
    }
}
