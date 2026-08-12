using EducAR.API.Controllers;
using EducAR.API.DTOs.Cursos;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class CursosControllerTests
{
    private readonly Mock<ICursoService> _serviceMock;
    private readonly CursosController _controller;

    public CursosControllerTests()
    {
        _serviceMock = new Mock<ICursoService>();
        _controller  = new CursosController(_serviceMock.Object);

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
        var filtro    = new FiltroCursoDto { Pagina = 1, Cantidad = 10 };
        var resultado = new ResultadoPaginadoDto<CursoResponseDto>
        {
            PaginaActual       = 1,
            TotalPaginas       = 1,
            TotalRegistros     = 2,
            RegistrosPorPagina = 10,
            Datos = new List<CursoResponseDto>
            {
                new() { IdCurso = 1, Grado = 1, Division = "A" },
                new() { IdCurso = 2, Grado = 1, Division = "B" }
            }
        };

        _serviceMock.Setup(s => s.ObtenerTodosPaginado(1, It.IsAny<FiltroCursoDto>()))
                    .ReturnsAsync(resultado);

        // Act
        var accion = await _controller.ObtenerTodos(filtro);

        // Assert
        var okResult = accion.Should().BeOfType<OkObjectResult>().Subject;
        var datos    = okResult.Value.Should().BeOfType<ResultadoPaginadoDto<CursoResponseDto>>().Subject;
        datos.TotalRegistros.Should().Be(2);
    }

    [Fact]
    public async Task Crear_CursoValido_Retorna201()
    {
        // Arrange
        var dto   = new CursoCreateDto { IdCicloLectivo = 1, Grado = 3, Division = "C", Turno = "Mañana" };
        var curso = new CursoResponseDto { IdCurso = 1, Grado = 3, Division = "C" };

        _serviceMock.Setup(s => s.Crear(dto, 1))
                    .ReturnsAsync((true, "Curso creado correctamente.", curso));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Eliminar_ConAlumnos_Retorna400()
    {
        // Arrange
        _serviceMock.Setup(s => s.Eliminar(1, 1))
                    .ReturnsAsync((false, "No se puede eliminar el curso porque tiene alumnos inscriptos activos."));

        // Act
        var accion = await _controller.Eliminar(1);

        // Assert
        accion.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Eliminar_SinDependencias_Retorna200()
    {
        // Arrange
        _serviceMock.Setup(s => s.Eliminar(1, 1))
                    .ReturnsAsync((true, "Curso dado de baja correctamente."));

        // Act
        var accion = await _controller.Eliminar(1);

        // Assert
        accion.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ObtenerPorId_NoExiste_Retorna404()
    {
        // Arrange
        _serviceMock.Setup(s => s.ObtenerPorId(99, 1)).ReturnsAsync((CursoResponseDto?)null);

        // Act
        var accion = await _controller.ObtenerPorId(99);

        // Assert
        accion.Should().BeOfType<NotFoundObjectResult>();
    }
}
