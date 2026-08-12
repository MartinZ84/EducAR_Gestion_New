using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IAsistenciaRepository
{
    Task<List<Asistencia>> ObtenerPorCursoYFecha(int idCurso, DateTime fecha);
    Task<List<Asistencia>> ObtenerPorAlumnoYCurso(int idAlumno, int idCurso);
    Task<bool> ExisteAsistenciaDelDia(int idCurso, DateTime fecha);
    Task RegistrarLote(List<Asistencia> asistencias);
    Task ActualizarLote(List<Asistencia> asistencias);
}