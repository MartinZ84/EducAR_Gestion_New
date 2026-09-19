using EducAR.API.DTOs.Asistencia;
using EducAR.API.Data;
using EducAR.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AsistenciaController : ControllerBase
{
    private readonly IAsistenciaService _asistenciaService;
    private readonly AppDbContext _context;

    public AsistenciaController(IAsistenciaService asistenciaService, AppDbContext context)
    {
        _asistenciaService = asistenciaService;
        _context = context;
    }

    private int IdEscuelaActual =>
        int.Parse(User.FindFirstValue("IdEscuela")!);

    private int IdUsuarioActual =>
        int.Parse(User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier)!);

    private Task<bool> PuedeVerCurso(int idCurso) => User.IsInRole("Administrador")
        ? Task.FromResult(true)
        : _context.DocenteMateriaCursos.AnyAsync(a => a.IdCurso == idCurso && a.Activo &&
            a.Docente.IdUsuario == IdUsuarioActual && a.Curso.IdEscuela == IdEscuelaActual);

    private IQueryable<Models.Curso> CursosPermitidos()
    {
        var query = _context.Cursos.Where(c => c.IdEscuela == IdEscuelaActual && c.Activo &&
            c.CicloLectivo.Activo && c.CicloLectivo.Anio == DateTime.Today.Year);
        if (!User.IsInRole("Administrador"))
            query = query.Where(c => c.DocenteMateriaCursos.Any(a => a.Activo && a.Docente.IdUsuario == IdUsuarioActual));
        return query;
    }

    [HttpGet("calendario/{idCurso}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerCalendario(int idCurso)
    {
        if (!await PuedeVerCurso(idCurso)) return Forbid();
        var curso = await CursosPermitidos().Include(c => c.CicloLectivo).FirstOrDefaultAsync(c => c.IdCurso == idCurso);
        if (curso is null) return NotFound(new { mensaje = "Curso activo no encontrado." });
        var fechas = await _context.Asistencias.Where(a => a.IdCurso == idCurso && a.Activo)
            .Select(a => a.Fecha.Date).Distinct().ToListAsync();
        var cargadas = fechas.ToHashSet();
        var dias = ConstruirDias(curso.CicloLectivo.FechaInicio.Date, curso.CicloLectivo.FechaFin.Date, cargadas);
        return Ok(new CalendarioAsistenciaDto
        {
            IdCurso = idCurso, Curso = $"{curso.Grado}° {curso.Division} ({curso.Turno})",
            FechaInicio = curso.CicloLectivo.FechaInicio.Date, FechaFin = curso.CicloLectivo.FechaFin.Date,
            Pendientes = dias.Count(d => d.Estado == "Pendiente"), Dias = dias
        });
    }

    [HttpGet("pendientes")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPendientes()
    {
        var cursos = await CursosPermitidos().Include(c => c.CicloLectivo).ToListAsync();
        var ids = cursos.Select(c => c.IdCurso).ToList();
        var registros = await _context.Asistencias.Where(a => ids.Contains(a.IdCurso) && a.Activo)
            .Select(a => new { a.IdCurso, Fecha = a.Fecha.Date }).Distinct().ToListAsync();
        var totales = cursos.Select(c => ConstruirDias(c.CicloLectivo.FechaInicio.Date, c.CicloLectivo.FechaFin.Date,
            registros.Where(a => a.IdCurso == c.IdCurso).Select(a => a.Fecha).ToHashSet()).Count(d => d.Estado == "Pendiente")).ToList();
        return Ok(new PendientesAsistenciaDto { Cantidad = totales.Sum(), CursosConPendientes = totales.Count(x => x > 0) });
    }

    // GET api/asistencia/curso/1?fecha=2026-07-01
    [HttpGet("curso/{idCurso}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerPorCursoYFecha(int idCurso, [FromQuery] DateTime fecha)
    {
        if (!await PuedeVerCurso(idCurso)) return Forbid();
        var resultado = await _asistenciaService.ObtenerPorCursoYFecha(idCurso, fecha, IdEscuelaActual);
        if (resultado is null) return NotFound(new { mensaje = "Curso no encontrado." });
        return Ok(resultado);
    }

    // GET api/asistencia/alumno/1/curso/1
    [HttpGet("alumno/{idAlumno}/curso/{idCurso}")]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> ObtenerResumenAlumno(int idAlumno, int idCurso)
    {
        if (!await PuedeVerCurso(idCurso)) return Forbid();
        var resultado = await _asistenciaService.ObtenerResumenAlumno(idAlumno, idCurso, IdEscuelaActual);
        return Ok(resultado);
    }

    // POST api/asistencia
    [HttpPost]
    [Authorize(Roles = "Administrador,Docente")]
    public async Task<IActionResult> Registrar([FromBody] AsistenciaRegistrarDto dto)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        // Obtener el IdDocente desde el IdUsuario del token
        var idUsuario = IdUsuarioActual;
        if (User.IsInRole("Administrador"))
        {
            var docenteAsignado = await _context.DocenteMateriaCursos
                .Where(a => a.IdCurso == dto.IdCurso && a.Activo && a.Docente.Usuario.Activo)
                .Select(a => (int?)a.Docente.IdUsuario).FirstOrDefaultAsync();
            if (!docenteAsignado.HasValue)
                return BadRequest(new { mensaje = "El curso debe tener al menos un docente asignado para registrar asistencia." });
            idUsuario = docenteAsignado.Value;
        }
        var (exito, mensaje) = await _asistenciaService.Registrar(dto, idUsuario, IdEscuelaActual);

        if (!exito) return BadRequest(new { mensaje });
        return Ok(new { mensaje });
    }

    private static List<DiaAsistenciaDto> ConstruirDias(DateTime inicio, DateTime fin, HashSet<DateTime> cargadas)
    {
        var hoy = DateTime.Today;
        var feriados = FeriadosArgentina(inicio.Year).Concat(inicio.Year == fin.Year ? new Dictionary<DateTime, string>() : FeriadosArgentina(fin.Year)).ToDictionary(x => x.Key, x => x.Value);
        var dias = new List<DiaAsistenciaDto>();
        for (var fecha = inicio; fecha <= fin; fecha = fecha.AddDays(1))
        {
            string estado; string? motivo = null;
            if (fecha.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday) { estado = "NoLaborable"; motivo = "Fin de semana"; }
            else if (feriados.TryGetValue(fecha, out var nombre)) { estado = "NoLaborable"; motivo = nombre; }
            else if (cargadas.Contains(fecha)) estado = "Cargada";
            else if (fecha <= hoy) estado = "Pendiente";
            else estado = "Futura";
            dias.Add(new DiaAsistenciaDto { Fecha = fecha, Estado = estado, Motivo = motivo });
        }
        return dias;
    }

    private static Dictionary<DateTime, string> FeriadosArgentina(int anio)
    {
        var f = new Dictionary<DateTime, string>();
        void Add(int mes, int dia, string nombre) => f[new DateTime(anio, mes, dia)] = nombre;
        Add(1,1,"Año Nuevo"); Add(3,24,"Día de la Memoria"); Add(4,2,"Día del Veterano y de los Caídos en Malvinas");
        Add(5,1,"Día del Trabajador"); Add(5,25,"Día de la Revolución de Mayo"); Add(6,20,"Día de la Bandera");
        Add(7,9,"Día de la Independencia"); Add(12,8,"Inmaculada Concepción"); Add(12,25,"Navidad");
        var pascua = Pascua(anio); f[pascua.AddDays(-2)] = "Viernes Santo";
        f[pascua.AddDays(-48)] = "Carnaval"; f[pascua.AddDays(-47)] = "Carnaval";
        f[MoverFeriado(new DateTime(anio,6,17))] = "Paso a la Inmortalidad de Güemes";
        f[MoverFeriado(new DateTime(anio,8,17))] = "Paso a la Inmortalidad de San Martín";
        f[MoverFeriado(new DateTime(anio,10,12))] = "Día del Respeto a la Diversidad Cultural";
        f[MoverFeriado(new DateTime(anio,11,20))] = "Día de la Soberanía Nacional";
        if (anio == 2026) { Add(3,23,"Día no laborable con fines turísticos"); Add(7,10,"Día no laborable con fines turísticos"); Add(12,7,"Día no laborable con fines turísticos"); }
        return f;
    }

    private static DateTime MoverFeriado(DateTime fecha) => fecha.DayOfWeek switch
    {
        DayOfWeek.Tuesday or DayOfWeek.Wednesday => fecha.AddDays(-(int)fecha.DayOfWeek + 1),
        DayOfWeek.Thursday or DayOfWeek.Friday => fecha.AddDays(8 - (int)fecha.DayOfWeek),
        _ => fecha
    };

    private static DateTime Pascua(int anio)
    {
        var a=anio%19; var b=anio/100; var c=anio%100; var d=b/4; var e=b%4; var g=(8*b+13)/25;
        var h=(19*a+b-d-g+15)%30; var j=c/4; var k=c%4; var m=(a+11*h)/319; var r=(2*e+2*j-k-h+m+32)%7;
        var mes=(h-m+r+90)/25; var dia=(h-m+r+mes+19)%32; return new DateTime(anio,mes,dia);
    }
}
