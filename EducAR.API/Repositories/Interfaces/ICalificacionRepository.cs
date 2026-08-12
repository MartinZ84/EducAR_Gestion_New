using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface ICalificacionRepository
{
    Task<List<Calificacion>> ObtenerPorCursoMateriaYPeriodo(int idCurso, int idMateria, int idPeriodo);
    Task<List<Calificacion>> ObtenerPorAlumno(int idAlumno, int idPeriodo);
    Task<Calificacion?> ObtenerPorAlumnoMateriaYPeriodo(int idAlumno, int idMateria, int idPeriodo);
    Task RegistrarLote(List<Calificacion> calificaciones);
    Task ActualizarLote(List<Calificacion> calificaciones);
}