using EducAR.API.DTOs.AlumnoTutor;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using EducAR.API.Services.Interfaces;
using EducAR.API.Data;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Services;

public class AlumnoTutorService : IAlumnoTutorService
{
    private readonly IAlumnoTutorRepository _alumnoTutorRepository;
    private readonly AppDbContext _context;

    public AlumnoTutorService(
        IAlumnoTutorRepository alumnoTutorRepository,
        AppDbContext context)
    {
        _alumnoTutorRepository = alumnoTutorRepository;
        _context               = context;
    }

    public async Task<List<AlumnoTutorResponseDto>> ObtenerPorAlumno(int idAlumno, int idEscuela)
    {
        // Validar que el alumno pertenece a la escuela
        var alumno = await _context.Alumnos
            .FirstOrDefaultAsync(a => a.IdAlumno == idAlumno && a.IdEscuela == idEscuela);
        if (alumno is null) return new List<AlumnoTutorResponseDto>();

        var relaciones = await _alumnoTutorRepository.ObtenerPorAlumno(idAlumno);
        return relaciones.Select(MapearAResponseDto).ToList();
    }

    public async Task<List<AlumnoTutorResponseDto>> ObtenerPorTutor(int idTutor, int idEscuela)
    {
        // Validar que el tutor pertenece a la escuela
        var tutor = await _context.Tutores
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.IdTutor == idTutor && t.Usuario.IdEscuela == idEscuela);
        if (tutor is null) return new List<AlumnoTutorResponseDto>();

        var relaciones = await _alumnoTutorRepository.ObtenerPorTutor(idTutor);
        return relaciones.Select(MapearAResponseDto).ToList();
    }

   

    public async Task<(bool exito, string mensaje, AlumnoTutorResponseDto? relacion)> Asociar(
        AlumnoTutorCreateDto dto, int idEscuela)
    {
        // Validar que el alumno pertenece a la escuela
        var alumno = await _context.Alumnos
            .FirstOrDefaultAsync(a => a.IdAlumno == dto.IdAlumno && a.IdEscuela == idEscuela);
        if (alumno is null)
            return (false, "El alumno no existe.", null);

        // Validar que el tutor pertenece a la escuela
        var tutor = await _context.Tutores
            .Include(t => t.Usuario)
            .FirstOrDefaultAsync(t => t.IdTutor == dto.IdTutor && t.Usuario.IdEscuela == idEscuela);
        if (tutor is null)
            return (false, "El tutor no existe.", null);

        // Validar que no existe ya la relación activa
        if (await _alumnoTutorRepository.ExisteRelacion(dto.IdAlumno, dto.IdTutor))
            return (false, "El tutor ya está asociado a este alumno.", null);

        // Validar responsable principal
        if (dto.EsResponsablePrinc && await _alumnoTutorRepository.TieneResponsablePrincipal(dto.IdAlumno))
            return (false, "El alumno ya tiene un responsable principal asignado. Desasocielo primero.", null);

        // Si existe una relación inactiva, reactivarla en lugar de crear una nueva
        var relacionInactiva = await _alumnoTutorRepository.ObtenerRelacionInactiva(dto.IdAlumno, dto.IdTutor);
        if (relacionInactiva is not null)
        {
            relacionInactiva.Parentesco         = dto.Parentesco;
            relacionInactiva.EsResponsablePrinc = dto.EsResponsablePrinc;
            var reactivada = await _alumnoTutorRepository.Reactivar(relacionInactiva);
            var completaReactivada = await _alumnoTutorRepository.ObtenerPorId(reactivada.IdAlumnoTutor);
            return (true, "Relación tutor-alumno reactivada correctamente.", MapearAResponseDto(completaReactivada!));
        }

        // Si no existe ninguna relación, crear una nueva
        var relacion = new AlumnoTutor
        {
            IdAlumno           = dto.IdAlumno,
            IdTutor            = dto.IdTutor,
            Parentesco         = dto.Parentesco,
            EsResponsablePrinc = dto.EsResponsablePrinc,
            Activo             = true
        };

        var creada = await _alumnoTutorRepository.Crear(relacion);
        var completa = await _alumnoTutorRepository.ObtenerPorId(creada.IdAlumnoTutor);
        return (true, "Tutor asociado al alumno correctamente.", MapearAResponseDto(completa!));
    }

    public async Task<(bool exito, string mensaje)> Desasociar(int idAlumnoTutor, int idEscuela)
    {
        var relacion = await _alumnoTutorRepository.ObtenerPorId(idAlumnoTutor);
        if (relacion is null)
            return (false, "Relación no encontrada.");

        if (relacion.Alumno.IdEscuela != idEscuela)
            return (false, "No tiene permisos para modificar esta relación.");

        var exito = await _alumnoTutorRepository.Eliminar(idAlumnoTutor, idEscuela);
        return exito
            ? (true, "Tutor desasociado del alumno correctamente.")
            : (false, "No se pudo desasociar el tutor.");
    }

    private static AlumnoTutorResponseDto MapearAResponseDto(AlumnoTutor at) => new()
    {
        IdAlumnoTutor      = at.IdAlumnoTutor,
        IdAlumno           = at.IdAlumno,
        NombreAlumno       = at.Alumno.Nombre,
        ApellidoAlumno     = at.Alumno.Apellido,
        IdTutor            = at.IdTutor,
        NombreTutor        = at.Tutor.Usuario.Nombre,
        ApellidoTutor      = at.Tutor.Usuario.Apellido,
        Parentesco         = at.Parentesco,
        EsResponsablePrinc = at.EsResponsablePrinc,
        Activo             = at.Activo
    };

    public async Task<List<AlumnoTutorResponseDto>> ObtenerMisAlumnos(int idUsuario, int idEscuela)
    {
        // Obtener el IdTutor a partir del IdUsuario del token
        var tutor = await _context.Tutores
            .FirstOrDefaultAsync(t => t.IdUsuario == idUsuario);

        if (tutor is null) return new List<AlumnoTutorResponseDto>();

        var relaciones = await _alumnoTutorRepository.ObtenerPorTutor(tutor.IdTutor);
        return relaciones.Select(MapearAResponseDto).ToList();
    }
}