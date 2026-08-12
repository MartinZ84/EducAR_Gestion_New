using EducAR.API.Controllers;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.DTOs.Tutores;
using EducAR.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class TutoresControllerTests
{
    private readonly Mock<ITutorService> _serviceMock;
    private readonly TutoresController _controller;

    public TutoresControllerTests()
    {
        _serviceMock = new Mock<ITutorService>();
        _controller  = new TutoresController(_serviceMock.Object);

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
        var resultado = new ResultadoPaginadoDto<TutorResponseDto>
        {
            PaginaActual       = 1,
            TotalPaginas       = 1,
            TotalRegistros     = 2,
            RegistrosPorPagina = 10,
            Datos = new List<TutorResponseDto>
            {
                new() { IdTutor = 1, Nombre = "Carlos", Apellido = "Fernández", Activo = true },
                new() { IdTutor = 2, Nombre = "María",  Apellido = "López",     Activo = true }
            }
        };

        _serviceMock.Setup(s => s.ObtenerTodosPaginado(1, It.IsAny<FiltroPersonaDto>()))
                    .ReturnsAsync(resultado);

        // Act
        var accion = await _controller.ObtenerTodos(filtro);

        // Assert
        var okResult = accion.Should().BeOfType<OkObjectResult>().Subject;
        var datos    = okResult.Value.Should().BeOfType<ResultadoPaginadoDto<TutorResponseDto>>().Subject;
        datos.TotalRegistros.Should().Be(2);
    }

    [Fact]
    public async Task Crear_DatosValidos_Retorna201()
    {
        // Arrange
        var dto = new TutorCreateDto
        {
            Dni           = 25000001,
            Nombre        = "Carlos",
            Apellido      = "Fernández",
            Email         = "carlos@test.com",
            NombreUsuario = "cfernandez",
            Contrasena    = "Tutor123",
            EsResponsable = true
        };
        var tutor = new TutorResponseDto { IdTutor = 1, Nombre = "Carlos", Apellido = "Fernández", Activo = true };

        _serviceMock.Setup(s => s.Crear(dto, 1))
                    .ReturnsAsync((true, "Tutor creado correctamente.", tutor));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Crear_DniDuplicado_Retorna400()
    {
        // Arrange
        var dto = new TutorCreateDto
        {
            Dni           = 12345678,
            Nombre        = "Carlos",
            Apellido      = "Fernández",
            Email         = "carlos@test.com",
            NombreUsuario = "cfernandez",
            Contrasena    = "Tutor123"
        };

        _serviceMock.Setup(s => s.Crear(dto, 1))
                    .ReturnsAsync((false, "Ya existe un usuario con ese DNI en esta escuela.", (TutorResponseDto?)null));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task ObtenerPorId_TutorExiste_Retorna200()
    {
        // Arrange
        var tutor = new TutorResponseDto { IdTutor = 1, Nombre = "Carlos", Apellido = "Fernández", Activo = true };
        _serviceMock.Setup(s => s.ObtenerPorId(1, 1)).ReturnsAsync(tutor);

        // Act
        var accion = await _controller.ObtenerPorId(1);

        // Assert
        accion.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_Retorna404()
    {
        // Arrange
        _serviceMock.Setup(s => s.ObtenerPorId(99, 1)).ReturnsAsync((TutorResponseDto?)null);

        // Act
        var accion = await _controller.ObtenerPorId(99);

        // Assert
        accion.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Eliminar_TutorExiste_Retorna200()
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
