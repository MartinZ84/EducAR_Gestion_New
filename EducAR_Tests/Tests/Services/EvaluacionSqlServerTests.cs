using EducAR.API.Data;
using EducAR.API.DTOs.Evaluaciones;
using EducAR.API.Models;
using EducAR.API.Repositories;
using EducAR.API.Services;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class SqlServerFactAttribute : FactAttribute
{
    public SqlServerFactAttribute()
    {
        if (Environment.GetEnvironmentVariable("EDUCAR_TEST_SQLSERVER") != "1")
            Skip = "Requiere SQL Server LocalDB y EDUCAR_TEST_SQLSERVER=1; crea y elimina una base aislada.";
    }
}

public class EvaluacionSqlServerTests
{
    [SqlServerFact]
    public async Task MigracionesYFlujoCompleto_EnBaseAislada()
    {
        var nombre = "EducAR_Evaluaciones_Test_" + Guid.NewGuid().ToString("N");
        var conexion = $"Server=(localdb)\\MSSQLLocalDB;Database={nombre};Integrated Security=true;TrustServerCertificate=true;Encrypt=false";
        await using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>().UseSqlServer(conexion).Options);
        try
        {
            // El historial antiguo presupone tablas externas (TelefonosContacto).
            // Construir la base anterior desde su modelo permite probar el upgrade real.
            var assembly = context.GetService<IMigrationsAssembly>();
            const string anterior = "20260915215612_AddEvaluacionesYNotas";
            var migration = assembly.CreateMigration(assembly.Migrations[anterior], context.Database.ProviderName!);
            await using (var previo = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(conexion).UseModel(migration.TargetModel).Options))
            {
                await previo.Database.EnsureCreatedAsync();
            }
            await context.Database.ExecuteSqlRawAsync(context.GetService<IHistoryRepository>().GetCreateScript());
            foreach (var version in assembly.Migrations.Keys.TakeWhile(id => string.CompareOrdinal(id, anterior) <= 0))
                await context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO __EFMigrationsHistory (MigrationId, ProductVersion) VALUES ({version}, {'1' + "0.0.9"})");
            await context.Database.MigrateAsync();
            var escuela = new Escuela { Nombre = "Prueba aislada", Direccion = "Prueba" };
            var ciclo = new CicloLectivo { Escuela = escuela, Anio = 2026, FechaInicio = new(2026, 1, 1), FechaFin = new(2026, 12, 31) };
            var curso = new Curso { Escuela = escuela, CicloLectivo = ciclo, Grado = 6, Division = "A" };
            var materia = new Materia { Escuela = escuela, Nombre = "Matemática" };
            var usuario = new Usuario { Escuela = escuela, Rol = new Rol { Nombre = "Docente" }, Nombre = "Docente", Apellido = "Prueba", Email = "test@example.invalid", NombreUsuario = "test", HashContrasena = "test" };
            var docente = new Docente { Usuario = usuario };
            var periodo = new PeriodoEvaluacion { CicloLectivo = ciclo, Nombre = "Primer trimestre", FechaInicio = new(2026, 3, 1), FechaFin = new(2026, 5, 31) };
            context.DocenteMateriaCursos.Add(new DocenteMateriaCurso { Curso = curso, Materia = materia, Docente = docente });
            context.PeriodosEvaluacion.Add(periodo);
            var alumnos = Enumerable.Range(1, 3).Select(i => new Alumno { Escuela = escuela, Nombre = "Alumno", Apellido = $"Prueba {i}", Dni = 40000000 + i }).ToList();
            foreach (var alumno in alumnos)
                context.Matriculas.Add(new Matricula { Escuela = escuela, Alumno = alumno, Curso = curso, CicloLectivo = ciclo });
            await context.SaveChangesAsync();
            var service = new EvaluacionService(context);
            var acceso = new AccesoEvaluacion(usuario.IdUsuario, escuela.IdEscuela, false);
            (await service.Opciones(acceso)).Estado.Should().Be(200);
            var crear = new EvaluacionCrearDto { IdCurso = curso.IdCurso, IdMateria = materia.IdMateria, IdPeriodoEvaluacion = periodo.IdPeriodoEvaluacion, Titulo = "Fracciones", Temario = "Operaciones", Descripcion = "Problemas", Fecha = new(2026, 4, 1) };
            (await service.Crear(crear, acceso)).Estado.Should().Be(200);
            var evaluacion = await context.Evaluaciones.SingleAsync();
            var inicial = System.Text.Json.JsonSerializer.SerializeToElement((await service.ObtenerAlumnos(evaluacion.IdEvaluacion, acceso)).Datos);
            inicial.GetArrayLength().Should().Be(3);
            var guardar = await service.GuardarNotas(evaluacion.IdEvaluacion, new() { Notas = new()
                { new() { IdAlumno = alumnos[0].IdAlumno, Valor = 7 }, new() { IdAlumno = alumnos[1].IdAlumno, Valor = 9 } } }, acceso);
            guardar.Estado.Should().Be(200);
            ((ResultadoNotas)guardar.Datos!).Resultados.Should().HaveCount(2);
            (await context.NotasEvaluacion.CountAsync()).Should().Be(2);
            (await context.Calificaciones.CountAsync()).Should().Be(2);
            (await service.GuardarNotas(evaluacion.IdEvaluacion, new() { Notas = new() { new() { IdAlumno = alumnos[0].IdAlumno, Valor = 8.5m } } }, acceso)).Estado.Should().Be(200);
            crear.Titulo = "Fracciones - recuperatorio";
            (await service.Editar(evaluacion.IdEvaluacion, crear, acceso)).Estado.Should().Be(200);
            (await service.Obtener(curso.IdCurso, materia.IdMateria, periodo.IdPeriodoEvaluacion, acceso)).Estado.Should().Be(200);
            context.ChangeTracker.Clear();
            (await context.NotasEvaluacion.SingleAsync(n => n.IdAlumno == alumnos[0].IdAlumno)).Valor.Should().Be(8.5m);
            (await context.NotasEvaluacion.AnyAsync(n => n.IdAlumno == alumnos[2].IdAlumno)).Should().BeFalse();

            var boletinService = new BoletinService(
                new BoletinRepository(context), new CursoRepository(context), new PeriodoEvaluacionRepository(context), context);
            var generado = await boletinService.GenerarParaAlumno(
                alumnos[0].IdAlumno, curso.IdCurso, periodo.IdPeriodoEvaluacion, escuela.IdEscuela);
            generado.exito.Should().BeTrue();
            generado.boletin!.Detalle.Single().CalificacionFinal.Should().Be(8.5m);
            var idBoletin = generado.boletin.IdBoletin;
            var guardado = await context.Boletines.SingleAsync(b => b.IdBoletin == idBoletin);
            guardado.ObservacionGeneral = "Observación conservada";
            await context.SaveChangesAsync();
            (await service.GuardarNotas(evaluacion.IdEvaluacion, new() { Notas = new()
                { new() { IdAlumno = alumnos[0].IdAlumno, Valor = 9.5m } } }, acceso)).Estado.Should().Be(200);
            var regenerado = await boletinService.GenerarParaAlumno(
                alumnos[0].IdAlumno, curso.IdCurso, periodo.IdPeriodoEvaluacion, escuela.IdEscuela);
            regenerado.exito.Should().BeTrue();
            regenerado.boletin!.IdBoletin.Should().Be(idBoletin);
            regenerado.boletin.Detalle.Single().CalificacionFinal.Should().Be(9.5m);
            regenerado.boletin.ObservacionGeneral.Should().Be("Observación conservada");
            (await context.Boletines.CountAsync()).Should().Be(1);

            var duplicado = () => context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO NotasEvaluacion (IdEvaluacion, IdAlumno, Valor, FechaAct) VALUES ({evaluacion.IdEvaluacion}, {alumnos[0].IdAlumno}, 7, GETDATE())");
            await duplicado.Should().ThrowAsync<SqlException>();
            var fueraDeRango = () => context.Database.ExecuteSqlInterpolatedAsync($"INSERT INTO NotasEvaluacion (IdEvaluacion, IdAlumno, Valor, FechaAct) VALUES ({evaluacion.IdEvaluacion}, {alumnos[2].IdAlumno}, 0, GETDATE())");
            await fueraDeRango.Should().ThrowAsync<SqlException>();
        }
        finally
        {
            // El nombre se genera aquí y nunca apunta a una base de la aplicación.
            await context.Database.EnsureDeletedAsync();
        }
    }
}
