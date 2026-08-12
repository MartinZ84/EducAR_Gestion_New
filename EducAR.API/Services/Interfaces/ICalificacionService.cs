using EducAR.API.DTOs.Calificaciones;

namespace EducAR.API.Services.Interfaces;

public interface ICalificacionService
{
    Task<CalificacionPorCursoResponseDto?> ObtenerPorCursoMateriaYPeriodo(int idCurso, int idMateria, int idPeriodo, int idEscuela);
    Task<List<CalificacionResponseDto>> ObtenerPorAlumno(int idAlumno, int idPeriodo, int idEscuela);
    Task<(bool exito, string mensaje)> Registrar(CalificacionRegistrarDto dto, int idEscuela);
}