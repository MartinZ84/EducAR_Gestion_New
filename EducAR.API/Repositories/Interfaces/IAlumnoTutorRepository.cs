using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IAlumnoTutorRepository
{
    Task<List<AlumnoTutor>> ObtenerPorAlumno(int idAlumno);
    Task<List<AlumnoTutor>> ObtenerPorTutor(int idTutor);
    Task<AlumnoTutor?> ObtenerPorId(int idAlumnoTutor);
    Task<bool> ExisteRelacion(int idAlumno, int idTutor);
    Task<bool> TieneResponsablePrincipal(int idAlumno);
    Task<AlumnoTutor> Crear(AlumnoTutor alumnoTutor);
    Task<bool> Eliminar(int idAlumnoTutor, int idEscuela);
    Task<AlumnoTutor?> ObtenerRelacionInactiva(int idAlumno, int idTutor);
    Task<AlumnoTutor> Reactivar(AlumnoTutor alumnoTutor);
}