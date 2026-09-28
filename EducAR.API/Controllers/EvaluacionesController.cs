using EducAR.API.DTOs.Evaluaciones;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/evaluaciones")]
[Authorize(Roles = "Administrador,Docente")]
public class EvaluacionesController(IEvaluacionService service) : ControllerBase
{
    private AccesoEvaluacion Acceso => new(
        int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!),
        int.Parse(User.FindFirstValue("IdEscuela")!), User.IsInRole("Administrador"));
    private IActionResult Responder(ResultadoEvaluacion resultado) =>
        resultado.Estado == 403 ? Forbid() : resultado.Estado == 200 ? Ok(resultado.Datos)
        : resultado.Estado == 400 ? BadRequest(resultado.Datos) : StatusCode(resultado.Estado, resultado.Datos);
    [HttpGet("opciones")]
    public async Task<IActionResult> Opciones() => Responder(await service.Opciones(Acceso));
    [HttpGet("curso/{idCurso}/materia/{idMateria}/periodo/{idPeriodo}")]
    public async Task<IActionResult> Obtener(int idCurso, int idMateria, int idPeriodo) =>
        Responder(await service.Obtener(idCurso, idMateria, idPeriodo, Acceso));
    [HttpPost]
    public async Task<IActionResult> Crear(EvaluacionCrearDto dto) => Responder(await service.Crear(dto, Acceso));
    [HttpPut("{idEvaluacion}")]
    public async Task<IActionResult> Editar(int idEvaluacion, EvaluacionEditarDto dto) =>
        Responder(await service.Editar(idEvaluacion, dto, Acceso));
    [HttpDelete("{idEvaluacion}")]
    public async Task<IActionResult> Archivar(int idEvaluacion) => Responder(await service.Archivar(idEvaluacion, Acceso));
    [HttpGet("{idEvaluacion}/alumnos")]
    public async Task<IActionResult> ObtenerAlumnos(int idEvaluacion) => Responder(await service.ObtenerAlumnos(idEvaluacion, Acceso));
    [HttpPut("{idEvaluacion}/notas")]
    public async Task<IActionResult> GuardarNotas(int idEvaluacion, NotasEvaluacionGuardarDto dto) =>
        Responder(await service.GuardarNotas(idEvaluacion, dto, Acceso));
}
