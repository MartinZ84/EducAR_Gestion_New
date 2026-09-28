using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EducAR.Tests.Tests.Services;

public class PeriodoEvaluacionDependenciasTests
{
    [Fact]
    public async Task NoPermiteBajaDePeriodoConEvaluaciones()
    {
        using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        context.CiclosLectivos.Add(new CicloLectivo { IdCicloLectivo = 1 });
        context.PeriodosEvaluacion.Add(new PeriodoEvaluacion { IdPeriodoEvaluacion = 1, IdCicloLectivo = 1, Nombre = "Primer trimestre" });
        context.Evaluaciones.Add(new Evaluacion { IdPeriodoEvaluacion = 1, Titulo = "Evaluación", Fecha = new DateTime(2026, 5, 1) });
        await context.SaveChangesAsync();
        var repo = new PeriodoEvaluacionRepository(context);
        (await repo.Eliminar(1, 1)).exito.Should().BeFalse();
        (await context.PeriodosEvaluacion.SingleAsync()).Activo.Should().BeTrue();
        (await repo.TieneEvaluacionesFueraDeFechas(1, new DateTime(2026, 3, 1), new DateTime(2026, 4, 1))).Should().BeTrue();
        (await repo.TieneEvaluacionesFueraDeFechas(1, new DateTime(2026, 3, 1), new DateTime(2026, 5, 1))).Should().BeFalse();
    }
}
