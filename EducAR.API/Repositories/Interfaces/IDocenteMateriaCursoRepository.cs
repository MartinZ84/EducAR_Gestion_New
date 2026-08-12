using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IDocenteMateriaCursoRepository
{
    Task<List<DocenteMateriaCurso>> ObtenerPorDocente(int idDocente);
    Task<List<DocenteMateriaCurso>> ObtenerPorCurso(int idCurso);
    Task<List<DocenteMateriaCurso>> ObtenerPorCursoYMateria(int idCurso, int idMateria);
    Task<DocenteMateriaCurso?> ObtenerPorId(int idDocenteMateriaCurso);
    Task<DocenteMateriaCurso?> ObtenerAsignacionActiva(int idDocente, int idMateria, int idCurso);
    Task<DocenteMateriaCurso?> ObtenerAsignacionInactiva(int idDocente, int idMateria, int idCurso);
    Task<bool> ExisteAsignacion(int idDocente, int idMateria, int idCurso);
    Task<DocenteMateriaCurso> Crear(DocenteMateriaCurso asignacion);
    Task<DocenteMateriaCurso> Reactivar(DocenteMateriaCurso asignacion);
    Task<bool> Eliminar(int idDocenteMateriaCurso, int idEscuela);
}