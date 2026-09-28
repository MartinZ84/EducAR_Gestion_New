using EducAR.API.Data;
using EducAR.API.DTOs.Mensajes;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services;
using EducAR.API.Services.Interfaces;
using EducAR.Tests.Helpers;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class MensajeServiceTests
{
    private readonly Mock<IMensajeRepository> _mensajeRepoMock;
    private readonly Mock<IEmailNotificacionesService> _emailNotificacionesServiceMock;
    private readonly AppDbContext _context;
    private readonly MensajeService _service;

    public MensajeServiceTests()
    {
        _mensajeRepoMock = new Mock<IMensajeRepository>();
        _emailNotificacionesServiceMock = new Mock<IEmailNotificacionesService>();
        _emailNotificacionesServiceMock
            .Setup(s => s.EnviarAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<IReadOnlyCollection<(string Email, string Nombre)>>(), It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: $"EducAR_Test_{Guid.NewGuid()}")
            .Options;

        _context = new AppDbContext(options);
        SeedDatabase();

        _service = new MensajeService(_mensajeRepoMock.Object, _context, _emailNotificacionesServiceMock.Object);
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
        var rol3 = TestDataBuilder.BuildRol(3, "Tutor");
        var usuario2 = TestDataBuilder.BuildUsuario(2, 2, 1, "docente", "Docente");
        var usuario3 = TestDataBuilder.BuildUsuario(3, 3, 1, "tutor", "Tutor");

        // REUTILIZAR las mismas instancias para evitar duplicados en el tracker de EF Core
        usuario1.Escuela = escuela;
        usuario1.Rol = rol1;
        usuario2.Escuela = escuela;
        usuario2.Rol = rol2;
        usuario3.Escuela = escuela;
        usuario3.Rol = rol3;

        _context.Escuelas.Add(escuela);
        _context.Roles.AddRange(rol1, rol2, rol3);
        _context.Usuarios.AddRange(usuario1, usuario2, usuario3);
        _context.Docentes.Add(new Docente { IdDocente = 1, IdUsuario = 2, Usuario = usuario2 });
        _context.Tutores.Add(new Tutor { IdTutor = 1, IdUsuario = 3, Usuario = usuario3 });
        _context.Alumnos.Add(new Alumno { IdAlumno = 1, IdEscuela = 1, Escuela = escuela, Nombre = "Alumno", Apellido = "Test", Activo = true });
        _context.CiclosLectivos.Add(new CicloLectivo { IdCicloLectivo = 1, IdEscuela = 1, Escuela = escuela, Anio = DateTime.Now.Year, FechaInicio = new DateTime(DateTime.Now.Year, 3, 1), FechaFin = new DateTime(DateTime.Now.Year, 12, 15), Activo = true });
        _context.Cursos.Add(new Curso { IdCurso = 1, IdEscuela = 1, IdCicloLectivo = 1, Escuela = escuela, Activo = true, Division = "A" });
        _context.Matriculas.Add(new Matricula { IdMatricula = 1, IdEscuela = 1, IdAlumno = 1, IdCurso = 1, IdCicloLectivo = 1, Estado = EstadoMatricula.Activa });
        _context.AlumnoTutores.Add(new AlumnoTutor { IdAlumnoTutor = 1, IdAlumno = 1, IdTutor = 1, Activo = true });
        _context.DocenteMateriaCursos.Add(new DocenteMateriaCurso { IdDocenteMateriaCurso = 1, IdDocente = 1, IdCurso = 1, IdMateria = 1, Activo = true });
        _context.SaveChanges();
    }

    [Fact]
    public async Task Enviar_DestinatarioValido_RetornaExito()
    {
        // Arrange
        var dto = new MensajeCreateDto
        {
            IdUsuarioDestinat = 3,
            Asunto = "Test",
            MensajeTexto = "Mensaje de prueba"
        };

        _mensajeRepoMock.Setup(r => r.Crear(It.IsAny<Mensaje>()))
                        .ReturnsAsync((Mensaje m) => { m.IdMensaje = 1; return m; });

        // Act
        var (exito, mensaje) = await _service.Enviar(dto, 2, 1);

        // Assert
        exito.Should().BeTrue();
        mensaje.Should().Be("Mensaje enviado correctamente.");
        _mensajeRepoMock.Verify(r => r.Crear(It.IsAny<Mensaje>()), Times.Once);
    }

    [Fact]
    public async Task Enviar_TutorVinculado_PuedeResponder()
    {
        _mensajeRepoMock.Setup(r => r.Crear(It.IsAny<Mensaje>()))
            .ReturnsAsync((Mensaje m) => m);
        var resultado = await _service.Enviar(new MensajeCreateDto
        {
            IdUsuarioDestinat = 2, Asunto = "Consulta", MensajeTexto = "¿Cómo está el alumno?"
        }, 3, 1);
        resultado.exito.Should().BeTrue();
    }

    [Fact]
    public async Task Enviar_VariosTutores_CreaUnMensajeParaCadaDestinatario()
    {
        var rolTutor = await _context.Roles.FindAsync(3);
        var escuela = await _context.Escuelas.FindAsync(1);
        var usuario = TestDataBuilder.BuildUsuario(4, 3, 1, "tutor2", "Tutor dos");
        usuario.Rol = rolTutor!; usuario.Escuela = escuela!;
        _context.Usuarios.Add(usuario);
        _context.Tutores.Add(new Tutor { IdTutor = 2, IdUsuario = 4, Usuario = usuario });
        _context.AlumnoTutores.Add(new AlumnoTutor { IdAlumnoTutor = 2, IdAlumno = 1, IdTutor = 2, Activo = true });
        await _context.SaveChangesAsync();
        _mensajeRepoMock.Setup(r => r.Crear(It.IsAny<Mensaje>())).ReturnsAsync((Mensaje m) => m);

        var resultado = await _service.Enviar(new MensajeCreateDto
        {
            IdsUsuariosDestinatarios = new() { 3, 4 }, Asunto = "Reunión", MensajeTexto = "Información"
        }, 2, 1);

        resultado.exito.Should().BeTrue();
        _mensajeRepoMock.Verify(r => r.Crear(It.IsAny<Mensaje>()), Times.Exactly(2));
    }

    [Fact]
    public async Task Admin_ObtieneDestinatariosDeSuEscuelaAgrupablesPorRol()
    {
        var rolAdmin = await _context.Roles.FindAsync(1);
        var rolTutor = await _context.Roles.FindAsync(3);
        var escuela = await _context.Escuelas.FindAsync(1);
        var otraEscuela = TestDataBuilder.BuildEscuela(2);
        var otroAdmin = TestDataBuilder.BuildUsuario(4, 1, 1, "admin2", "Otro admin");
        var usuarioOtraEscuela = TestDataBuilder.BuildUsuario(5, 3, 2, "tutor-otra-escuela", "Tutor");
        otroAdmin.Rol = rolAdmin!;
        otroAdmin.Escuela = escuela!;
        usuarioOtraEscuela.Rol = rolTutor!;
        usuarioOtraEscuela.Escuela = otraEscuela;
        _context.Escuelas.Add(otraEscuela);
        _context.Usuarios.AddRange(otroAdmin, usuarioOtraEscuela);
        await _context.SaveChangesAsync();

        var destinatarios = await _service.ObtenerDestinatarios(1, 1);

        destinatarios.Should().HaveCount(3);
        destinatarios.Select(d => d.Rol).Should().BeEquivalentTo("Administrador", "Docente", "Tutor");
        destinatarios.Should().NotContain(d => d.IdUsuario == 1);
        destinatarios.Should().NotContain(d => d.IdUsuario == 5);
    }

    [Fact]
    public async Task Admin_PuedeEnviarAUnoOMasRolesDeSuEscuela()
    {
        var rolAdmin = await _context.Roles.FindAsync(1);
        var escuela = await _context.Escuelas.FindAsync(1);
        var otroAdmin = TestDataBuilder.BuildUsuario(4, 1, 1, "admin2", "Otro admin");
        otroAdmin.Rol = rolAdmin!;
        otroAdmin.Escuela = escuela!;
        _context.Usuarios.Add(otroAdmin);
        await _context.SaveChangesAsync();
        _mensajeRepoMock.Setup(r => r.Crear(It.IsAny<Mensaje>())).ReturnsAsync((Mensaje m) => m);

        var resultado = await _service.Enviar(new MensajeCreateDto
        {
            IdsUsuariosDestinatarios = new() { 2, 3, 4 },
            Asunto = "Aviso general",
            MensajeTexto = "Información para docentes y tutores"
        }, 1, 1);

        resultado.exito.Should().BeTrue();
        _mensajeRepoMock.Verify(r => r.Crear(It.IsAny<Mensaje>()), Times.Exactly(3));
    }

    [Fact]
    public async Task ObtenerDestinatarios_ListaUnaOpcionPorAlumnoAunqueCompartanTutor()
    {
        _context.Alumnos.Add(new Alumno
            { IdAlumno = 2, IdEscuela = 1, Nombre = "Segundo", Apellido = "Alumno", Activo = true });
        _context.Matriculas.Add(new Matricula
            { IdMatricula = 2, IdEscuela = 1, IdAlumno = 2, IdCurso = 1, IdCicloLectivo = 1, Estado = EstadoMatricula.Activa });
        _context.AlumnoTutores.Add(new AlumnoTutor
            { IdAlumnoTutor = 2, IdAlumno = 2, IdTutor = 1, Activo = true });
        await _context.SaveChangesAsync();

        var destinatarios = await _service.ObtenerDestinatarios(2, 1);

        destinatarios.Should().HaveCount(2);
        destinatarios.Should().OnlyContain(d => d.IdUsuario == 3 && d.Rol == "Tutor");
        destinatarios.Select(d => d.IdAlumno).Should().BeEquivalentTo(new[] { 1, 2 });
        destinatarios.Select(d => d.NombreAlumno).Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task Enviar_CicloAnterior_RechazaAunqueExisteVinculo()
    {
        var ciclo = await _context.CiclosLectivos.FindAsync(1);
        ciclo!.Anio = DateTime.Now.Year - 1;
        await _context.SaveChangesAsync();

        var resultado = await _service.Enviar(new MensajeCreateDto
        {
            IdUsuarioDestinat = 3, Asunto = "Consulta", MensajeTexto = "Hola"
        }, 2, 1);
        resultado.exito.Should().BeFalse();
        _mensajeRepoMock.Verify(r => r.Crear(It.IsAny<Mensaje>()), Times.Never);
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

}
