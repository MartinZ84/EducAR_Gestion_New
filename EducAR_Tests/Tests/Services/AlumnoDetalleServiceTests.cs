using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories;
using EducAR.API.Services;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class AlumnoDetalleServiceTests
{
    [Fact]
    public async Task ObtenerDetalle_ConMatricula_CargaElCicloLectivo()
    {
        var nombreBase = Guid.NewGuid().ToString();
        var opciones = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(nombreBase).Options;

        await using (var seed = new AppDbContext(opciones))
        {
            seed.Escuelas.Add(new Escuela { IdEscuela = 1, Nombre = "Escuela", Direccion = "Dirección" });
            seed.Alumnos.Add(new Alumno { IdAlumno = 1, IdEscuela = 1, Dni = 12345678, Nombre = "Ana", Apellido = "Pérez" });
            seed.CiclosLectivos.Add(new CicloLectivo { IdCicloLectivo = 1, IdEscuela = 1, Anio = 2026, Activo = true });
            seed.Cursos.Add(new Curso { IdCurso = 1, IdEscuela = 1, IdCicloLectivo = 1, Grado = 3, Division = "A", Activo = true });
            seed.Matriculas.Add(new Matricula { IdMatricula = 1, IdEscuela = 1, IdAlumno = 1, IdCurso = 1, IdCicloLectivo = 1, Estado = EstadoMatricula.Activa });
            await seed.SaveChangesAsync();
        }

        await using var contexto = new AppDbContext(opciones);
        var servicio = new AlumnoService(new AlumnoRepository(contexto));

        var detalle = await servicio.ObtenerDetalle(1, 1);

        detalle.Should().NotBeNull();
        detalle!.MatriculaActual.Should().NotBeNull();
        detalle.MatriculaActual!.CicloLectivo.Should().Be("2026");
        detalle.MatriculaActual.Curso.Should().Contain("3").And.Contain("A");
    }
}
