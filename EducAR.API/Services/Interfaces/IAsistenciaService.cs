using EducAR.API.DTOs.Asistencia;

namespace EducAR.API.Services.Interfaces;

public interface IAsistenciaService
{
    Task<AsistenciaPorFechaResponseDto?> ObtenerPorCursoYFecha(int idCurso, DateTime fecha, int idEscuela);
    Task<List<AsistenciaAlumnoResumenDto>> ObtenerResumenAlumno(int idAlumno, int idCurso, int idEscuela);
    Task<(bool exito, string mensaje)> Registrar(AsistenciaRegistrarDto dto, int idDocente, int idEscuela);
}