using EducAR.API.Controllers;
using EducAR.API.DTOs.Matriculas;
using EducAR.API.DTOs.Paginacion;
using EducAR.API.Services.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System.Security.Claims;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class MatriculasControllerTests
{
    private readonly Mock<IMatriculaService> _serviceMock;
    private readonly MatriculasController _controller;

    public MatriculasControllerTests()
    {
        _serviceMock = new Mock<IMatriculaService>();
        _controller = new MatriculasController(_serviceMock.Object);

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
    public async Task ObtenerAlumnosDisponibles_RetornaOkConPaginacion()
    {
        var resultado = new ResultadoPaginadoDto<MatriculaAlumnoDisponibleDto>
        {
            PaginaActual = 1,
            TotalPaginas = 1,
            TotalRegistros = 1,
            RegistrosPorPagina = 25,
            Datos = new()
            {
                new()
                {
                    IdAlumno = 1,
                    Dni = 40123456,
                    Apellido = "Pérez",
                    Nombre = "Juan",
                    FecNac = new DateTime(2014, 3, 12)
                }
            }
        };

        _serviceMock
            .Setup(s => s.ObtenerAlumnosParaMatricular(1, 2026, 1, It.IsAny<PaginacionDto>(), null, null, null))
            .ReturnsAsync(resultado);

        var accion = await _controller.ObtenerAlumnosDisponibles(
            2026,
            1,
            new PaginacionDto { Pagina = 1, Cantidad = 25 },
            null,
            null,
            null);

        var ok = accion.Should().BeOfType<OkObjectResult>().Subject;
        ok.Value.Should().Be(resultado);
    }

    [Fact]
    public async Task AsignarMasivo_Valido_Retorna200()
    {
        var dto = new MatriculaAsignacionMasivaDto
        {
            IdCurso = 1,
            IdsAlumnos = new List<int> { 1, 2, 3 }
        };

        var resultado = new MatriculaAsignacionResultadoDto
        {
            TotalSolicitados = 3,
            Matriculados = 3
        };

        _serviceMock
            .Setup(s => s.AsignarMasivo(dto, 1))
            .ReturnsAsync((true, "Proceso finalizado.", resultado));

        var accion = await _controller.AsignarMasivo(dto);

        accion.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task DarDeBaja_NoExiste_Retorna404()
    {
        _serviceMock
            .Setup(s => s.DarDeBaja(99, 1))
            .ReturnsAsync((false, "La matrícula no existe o no está activa."));

        var accion = await _controller.DarDeBaja(99);

        accion.Should().BeOfType<NotFoundObjectResult>();
    }
}
