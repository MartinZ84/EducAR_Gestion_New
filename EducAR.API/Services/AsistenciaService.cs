using EducAR.API.DTOs.Asistencia;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;

namespace EducAR.API.Services;

public class AsistenciaService : IAsistenciaService
{
    private readonly IAsistenciaRepository _asistenciaRepository;
    private readonly ICursoRepository _cursoRepository;
    private readonly IDocenteRepository _docenteRepository;

    public AsistenciaService(
        IAsistenciaRepository asistenciaRepository,
        ICursoRepository cursoRepository,
        IDocenteRepository docenteRepository)
    {
        _asistenciaRepository = asistenciaRepository;
        _cursoRepository = cursoRepository;
        _docenteRepository = docenteRepository;
    }

    public async Task<AsistenciaPorFechaResponseDto?> ObtenerPorCursoYFecha(int idCurso, DateTime fecha, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return null;

        var asistencias = await _asistenciaRepository.ObtenerPorCursoYFecha(idCurso, fecha);

        return new AsistenciaPorFechaResponseDto
        {
            Fecha        = fecha,
            IdCurso      = idCurso,
            TotalAlumnos = asistencias.Count,
            Presentes    = asistencias.Count(a => a.Presente),
            Ausentes     = asistencias.Count(a => !a.Presente),
            Detalle      = asistencias.Select(MapearAResponseDto).ToList()
        };
    }

    public async Task<List<AsistenciaAlumnoResumenDto>> ObtenerResumenAlumno(int idAlumno, int idCurso, int idEscuela)
    {
        var curso = await _cursoRepository.ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return new List<AsistenciaAlumnoResumenDto>();

        var asistencias = await _asistenciaRepository.ObtenerPorAlumnoYCurso(idAlumno, idCurso);
        if (!asistencias.Any()) return new List<AsistenciaAlumnoResumenDto>();

        var alumno = asistencias.First().Alumno;
        var total  = asistencias.Count;
        var presentes = asistencias.Count(a => a.Presente);

        return new List<AsistenciaAlumnoResumenDto>
        {
            new()
            {
                IdAlumno             = idAlumno,
                NombreCompleto       = $"{alumno.Apellido}, {alumno.Nombre}",
                TotalDias            = total,
                Presentes            = presentes,
                Ausentes             = total - presentes,
                PorcentajeAsistencia = total > 0
                    ? Math.Round((decimal)presentes / total * 100, 2)
                    : 0
            }
        };
    }

    public async Task<(bool exito, string mensaje)> Registrar(AsistenciaRegistrarDto dto, int idDocente, int idEscuela)
    {     
        // Validar que el curso existe y pertenece a la escuela
        var curso = await _cursoRepository.ObtenerPorId(dto.IdCurso, idEscuela);
        if (curso is null)
            return (false, "El curso no existe.");

        if (!curso.Activo)
            return (false, "El curso no está activo.");

        // Obtener el IdDocente a partir del IdUsuario
        var docentes = await _docenteRepository.ObtenerTodos(idEscuela);
        // VALIDAR QUE docentes NO SEA NULL
        if (docentes == null)
            return (false, "No se encontraron docentes para la escuela.");
        var docente = docentes.FirstOrDefault(d => d.IdUsuario == idDocente);
        if (docente is null)
            return (false, "No se encontró el docente asociado al usuario.");

        var idDocenteReal = docente.IdDocente;

        if (!dto.Alumnos.Any())
            return (false, "Debe incluir al menos un alumno.");

        var fechaSinHora = dto.Fecha.Date;

        // Si ya existe asistencia del día → actualizar, sino → crear
        var yaExiste = await _asistenciaRepository.ExisteAsistenciaDelDia(dto.IdCurso, fechaSinHora);

        if (yaExiste)
        {
            var existentes = await _asistenciaRepository.ObtenerPorCursoYFecha(dto.IdCurso, fechaSinHora);
            foreach (var item in dto.Alumnos)
            {
                var reg = existentes.FirstOrDefault(a => a.IdAlumno == item.IdAlumno);
                if (reg is not null)
                {
                    reg.Presente  = item.Presente;
                    reg.FechaAct  = DateTime.Now;
                }
            }
            await _asistenciaRepository.ActualizarLote(existentes);
            return (true, "Asistencia actualizada correctamente.");
        }

        var nuevas = dto.Alumnos.Select(a => new Asistencia
        {
            IdDocente = idDocenteReal,
            IdAlumno  = a.IdAlumno,
            IdCurso   = dto.IdCurso,
            Fecha     = fechaSinHora,
            Presente  = a.Presente,
            Activo    = true
        }).ToList();

        await _asistenciaRepository.RegistrarLote(nuevas);
        return (true, "Asistencia registrada correctamente.");
    }

    private static AsistenciaResponseDto MapearAResponseDto(Asistencia a) => new()
    {
        IdAsistencia   = a.IdAsistencia,
        IdAlumno       = a.IdAlumno,
        NombreAlumno   = a.Alumno.Nombre,
        ApellidoAlumno = a.Alumno.Apellido,
        IdCurso        = a.IdCurso,
        Fecha          = a.Fecha,
        Presente       = a.Presente
    };
}