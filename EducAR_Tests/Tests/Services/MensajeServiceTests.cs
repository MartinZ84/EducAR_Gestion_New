using EducAR.API.Data;
using EducAR.API.DTOs.Mensajes;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class MensajeServiceTests
{
    private readonly Mock<IMensajeRepository> _mensajeRepoMock;
    private readonly AppDbContext _context;
    private readonly MensajeService _service;

    public MensajeServiceTests()
    {
        _mensajeRepoMock = new Mock<IMensajeRepository>();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"EducAR_Test_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        SeedDatabase();

        _service = new MensajeService(_mensajeRepoMock.Object, _context);
    }

    // private void SeedDatabase()
    // {
    //     var escuela = TestDataBuilder.BuildEscuela();
    //     var rol1    = TestDataBuilder.BuildRol(1, "Administrador");
    //     var rol2    = TestDataBuilder.BuildRol(2, "Docente");

    //     var usuario1 = TestDataBuilder.BuildUsuario(1, 1, 1, "admin",   "Administrador");
    //     var usuario2 = TestDataBuilder.BuildUsuario(2, 2, 1, "docente", "Docente");

    //     _context.Escuelas.Add(escuela);
    //     _context.Roles.AddRange(rol1, rol2);
    //     _context.Usuarios.AddRange(usuario1, usuario2);
    //     _context.SaveChanges();

    // }
    private void SeedDatabase()
    {
        var escuela = TestDataBuilder.BuildEscuela();
        var rol1 = TestDataBuilder.BuildRol(1, "Administrador");
        var rol2 = TestDataBuilder.BuildRol(2, "Docente");

        var usuario1 = TestDataBuilder.BuildUsuario(1, 1, 1, "admin", "Administrador");
        var usuario2 = TestDataBuilder.BuildUsuario(2, 2, 1, "docente", "Docente");

        // REUTILIZAR las mismas instancias para evitar duplicados en el tracker de EF Core
        usuario1.Escuela = escuela;
        usuario1.Rol = rol1;
        usuario2.Escuela = escuela;
        usuario2.Rol = rol2;

        _context.Escuelas.Add(escuela);
        _context.Roles.AddRange(rol1, rol2);
        _context.Usuarios.AddRange(usuario1, usuario2);
        _context.SaveChanges();
    }

    [Fact]
    public async Task Enviar_DestinatarioValido_RetornaExito()
    {
        // Arrange
        var dto = new MensajeCreateDto
        {
            IdUsuarioDestinat = 2,
            Asunto = "Test",
            MensajeTexto = "Mensaje de prueba"
        };

        _mensajeRepoMock.Setup(r => r.Crear(It.IsAny<Mensaje>()))
                        .ReturnsAsync((Mensaje m) => { m.IdMensaje = 1; return m; });

        // Act
        var (exito, mensaje) = await _service.Enviar(dto, 1, 1);

        // Assert
        exito.Should().BeTrue();
        mensaje.Should().Be("Mensaje enviado correctamente.");
        _mensajeRepoMock.Verify(r => r.Crear(It.IsAny<Mensaje>()), Times.Once);
    }

    [Fact]
    public async Task Enviar_DestinatarioNoExiste_RetornaError()
    {
        // Arrange
        var dto = new MensajeCreateDto
        {
            IdUsuarioDestinat = 99,
            Asunto = "Test",
            MensajeTexto = "Mensaje"
        };

        // Act
        var (exito, mensaje) = await _service.Enviar(dto, 1, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("no existe");
    }

    [Fact]
    public async Task Enviar_MismoRemitenteYDestinatario_RetornaError()
    {
        // Arrange
        var dto = new MensajeCreateDto
        {
            IdUsuarioDestinat = 1, // mismo que el remitente
            Asunto = "Test",
            MensajeTexto = "Mensaje"
        };

        // Act
        var (exito, mensaje) = await _service.Enviar(dto, 1, 1);

        // Assert
        exito.Should().BeFalse();
        mensaje.Should().Contain("mismo");
    }

    [Fact]
    public async Task ObtenerRecibidos_RetornaListaDelUsuario()
    {
        // Arrange
        var mensajes = new List<Mensaje>
        {
            new()
            {
                IdMensaje          = 1,
                IdUsuarioRemitente = 1,
                IdUsuarioDestinat  = 2,
                Asunto             = "Asunto 1",
                MensajeTexto       = "Texto 1",
                FechaEnvio         = DateTime.Now,
                Leido              = false,
                Activo             = true,
                Remitente          = TestDataBuilder.BuildUsuario(1),
                Destinatario       = TestDataBuilder.BuildUsuario(2, 2, 1, "docente", "Docente")
            }
        };

        _mensajeRepoMock.Setup(r => r.ObtenerRecibidos(2)).ReturnsAsync(mensajes);

        // Act
        var resultado = await _service.ObtenerRecibidos(2);

        // Assert
        resultado.Should().HaveCount(1);
        resultado[0].Asunto.Should().Be("Asunto 1");
    }

    [Fact]
    public async Task MarcarLeido_MensajeExiste_RetornaTrue()
    {
        // Arrange
        _mensajeRepoMock.Setup(r => r.MarcarLeido(1, 2)).ReturnsAsync(true);

        // Act
        var resultado = await _service.MarcarLeido(1, 2);

        // Assert
        resultado.Should().BeTrue();
    }

    [Fact]
    public async Task MarcarLeido_MensajeNoExiste_RetornaFalse()
    {
        // Arrange
        _mensajeRepoMock.Setup(r => r.MarcarLeido(99, 2)).ReturnsAsync(false);

        // Act
        var resultado = await _service.MarcarLeido(99, 2);

        // Assert
        resultado.Should().BeFalse();
    }

    [Fact]
    public async Task ContarNoLeidos_RetornaCantidad()
    {
        // Arrange
        _mensajeRepoMock.Setup(r => r.ContarNoLeidos(2)).ReturnsAsync(3);

        // Act
        var resultado = await _service.ContarNoLeidos(2);

        // Assert
        resultado.Should().Be(3);
    }

    [Fact]
    public async Task Eliminar_MensajeExiste_RetornaTrue()
    {
        // Arrange
        _mensajeRepoMock.Setup(r => r.Eliminar(1, 2)).ReturnsAsync(true);

        // Act
        var resultado = await _service.Eliminar(1, 2);

        // Assert
        resultado.Should().BeTrue();
    }
}
