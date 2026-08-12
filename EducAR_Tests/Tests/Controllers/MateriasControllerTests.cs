using EducAR.API.Controllers;
using EducAR.API.DTOs.Materias;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class MateriasControllerTests
{
    private readonly Mock<IMateriaService> _serviceMock;
    private readonly MateriasController _controller;

    public MateriasControllerTests()
    {
        _serviceMock = new Mock<IMateriaService>();
        _controller  = new MateriasController(_serviceMock.Object);

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
    public async Task ObtenerTodas_RetornaOkConPaginacion()
    {
        // Arrange
        var filtro    = new FiltroNombreDto { Pagina = 1, Cantidad = 10 };
        var resultado = new ResultadoPaginadoDto<MateriaResponseDto>
        {
            PaginaActual       = 1,
            TotalPaginas       = 1,
            TotalRegistros     = 3,
            RegistrosPorPagina = 10,
            Datos = new List<MateriaResponseDto>
            {
                new() { IdMateria = 1, Nombre = "Matemática" },
                new() { IdMateria = 2, Nombre = "Lengua" },
                new() { IdMateria = 3, Nombre = "Ciencias" }
            }
        };

        _serviceMock.Setup(s => s.ObtenerTodasPaginado(1, It.IsAny<FiltroNombreDto>()))
                    .ReturnsAsync(resultado);

        // Act
        var accion = await _controller.ObtenerTodas(filtro);

        // Assert
        var okResult = accion.Should().BeOfType<OkObjectResult>().Subject;
        var datos    = okResult.Value.Should().BeOfType<ResultadoPaginadoDto<MateriaResponseDto>>().Subject;
        datos.TotalRegistros.Should().Be(3);
    }

    [Fact]
    public async Task Crear_NombreValido_Retorna201()
    {
        // Arrange
        var dto     = new MateriaCreateDto { Nombre = "Física" };
        var materia = new MateriaResponseDto { IdMateria = 1, Nombre = "Física", Activo = true };

        _serviceMock.Setup(s => s.Crear(dto, 1))
                    .ReturnsAsync((true, "Materia creada correctamente.", materia));

        // Act
        var accion = await _controller.Crear(dto);

        // Assert
        accion.Should().BeOfType<CreatedAtActionResult>();
    }

    [Fact]
    public async Task Eliminar_MateriaExiste_Retorna200()
    {
        // Arrange
        _serviceMock.Setup(s => s.Eliminar(1, 1)).ReturnsAsync(true);

        // Act
        var accion = await _controller.Eliminar(1);

        // Assert
        accion.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Eliminar_MateriaNoExiste_Retorna404()
    {
        // Arrange
        _serviceMock.Setup(s => s.Eliminar(99, 1)).ReturnsAsync(false);

        // Act
        var accion = await _controller.Eliminar(99);

        // Assert
        accion.Should().BeOfType<NotFoundObjectResult>();
    }
}
