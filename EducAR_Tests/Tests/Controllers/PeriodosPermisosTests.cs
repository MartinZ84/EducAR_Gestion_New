using EducAR.API.Controllers;
using Microsoft.AspNetCore.Authorization;
using FluentAssertions;
using Xunit;

namespace EducAR.Tests.Tests.Controllers;

public class PeriodosPermisosTests
{
    [Theory]
    [InlineData(nameof(PeriodosEvaluacionController.ObtenerTodos), "Administrador,Docente")]
    [InlineData(nameof(PeriodosEvaluacionController.ObtenerPorId), "Administrador,Docente")]
    [InlineData(nameof(PeriodosEvaluacionController.Crear), "Administrador")]
    [InlineData(nameof(PeriodosEvaluacionController.Actualizar), "Administrador")]
    [InlineData(nameof(PeriodosEvaluacionController.Eliminar), "Administrador")]
    public void Endpoints_RestringenRoles(string metodo, string roles)
    {
        var atributos = typeof(PeriodosEvaluacionController).GetMethod(metodo)!
            .GetCustomAttributes(typeof(AuthorizeAttribute), true).Cast<AuthorizeAttribute>();
        atributos.Should().ContainSingle(a => a.Roles == roles);
        typeof(PeriodosEvaluacionController).GetMethod(metodo)!
            .GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Should().BeEmpty();
    }
}
