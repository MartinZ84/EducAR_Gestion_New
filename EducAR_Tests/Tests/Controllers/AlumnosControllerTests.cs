using EducAR.API.Controllers;
using EducAR.API.DTOs.Alumnos;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class AlumnosControllerTests
{
    private readonly Mock<IAlumnoService> _serviceMock;
    private readonly AlumnosController _controller;

    public AlumnosControllerTests()
    {
        _serviceMock = new Mock<IAlumnoService>();
        _controller  = new AlumnosController(_serviceMock.Object);

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
        var resultado = new ResultadoPaginadoDto<AlumnoResponseDto>
        {
            PaginaActual       = 1,
            TotalPaginas       = 1,
            TotalRegistros     = 3,
            RegistrosPorPagina = 10,
            Datos = new List<AlumnoResponseDto>
            {
                new() { IdAlumno = 1, Nombre = "Juan",  Apellido = "García" },
                new() { IdAlumno = 2, Nombre = "María", Apellido = "López" },
                new() { IdAlumno = 3, Nombre = "Pedro", Apellido = "Martínez" }
            }
        };

        _serviceMock.Setup(s => s.ObtenerTodosPaginado(1, It.IsAny<FiltroPersonaDto>()))
                    .ReturnsAsync(resultado);

        // Act
        var accion = await _controller.ObtenerTodos(filtro);

        // Assert
        var okResult = accion.Should().BeOfType<OkObjectResult>().Subject;
        var datos    = okResult.Value.Should().BeOfType<ResultadoPaginadoDto<AlumnoResponseDto>>().Subject;
        datos.TotalRegistros.Should().Be(3);
    }

    [Fact]
    public async Task ObtenerPorId_AlumnoExiste_Retorna200()
    {
        // Arrange
        var alumno = new AlumnoResponseDto { IdAlumno = 1, Nombre = "Juan", Apellido = "García" };
        _serviceMock.Setup(s => s.ObtenerPorId(1, 1)).ReturnsAsync(alumno);

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
        _serviceMock.Setup(s => s.ObtenerPorId(99, 1)).ReturnsAsync((AlumnoResponseDto?)null);

        // Act
        var accion = await _controller.ObtenerPorId(99);

        // Assert
        accion.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Crear_DatosValidos_Retorna201()
    {
        // Arrange
        var dto    = new AlumnoCreateDto { Dni = 40000001, Nombre = "Juan", Apellido = "García", FechaNacimiento = new DateTime(2014, 1, 1) };
        var alumno = new AlumnoResponseDto { IdAlumno = 1, Nombre = "Juan", Apellido = "García" };

        _serviceMock.Setup(s => s.Crear(dto, 1))
                    .ReturnsAsync((true, "Alumno creado correctamente.", alumno));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Crear_DniDuplicado_Retorna400()
    {
        // Arrange
        var dto = new AlumnoCreateDto { Dni = 12345678, Nombre = "Juan", Apellido = "García", FechaNacimiento = new DateTime(2014, 1, 1) };

        _serviceMock.Setup(s => s.Crear(dto, 1))
                    .ReturnsAsync((false, "Ya existe un alumno con ese DNI en esta escuela.", (AlumnoResponseDto?)null));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Eliminar_AlumnoExiste_Retorna200()
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
