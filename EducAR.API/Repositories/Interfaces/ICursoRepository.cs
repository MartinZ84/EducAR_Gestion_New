using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface ICursoRepository
{
    Task<List<Curso>> ObtenerTodos(int idEscuela);
    Task<List<Curso>> ObtenerPorCicloLectivo(int idCicloLectivo, int idEscuela);
    Task<Curso?> ObtenerPorId(int idCurso, int idEscuela);
    Task<Curso?> ObtenerDetalle(int idCurso, int idEscuela);
    Task<bool> ExisteCurso(int grado, string division, string? turno, int idCicloLectivo, int? excluirId = null);
    Task<bool> TieneAlumnosInscriptos(int idCurso);
    Task<bool> TieneAsistencias(int idCurso);
    Task<bool> TieneCalificaciones(int idCurso);
    Task<Curso> Crear(Curso curso);
    Task<bool> Actualizar(Curso curso);
    Task<bool> Eliminar(int idCurso, int idEscuela);
    Task<IQueryable<Curso>> ObtenerQueryable(int idEscuela, int? grado = null, string? division = null, string? turno = null, int? idCicloLectivo = null);
}
