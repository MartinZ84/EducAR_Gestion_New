using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IAlumnoRepository
{
    Task<List<Alumno>> ObtenerTodos(int idEscuela);
    Task<Alumno?> ObtenerPorId(int idAlumno, int idEscuela);
    Task<bool> ExisteDni(int dni, int idEscuela, int? excluirIdAlumno = null);
    Task<Alumno> Crear(Alumno alumno);
    Task<bool> Actualizar(Alumno alumno);
    Task<bool> Eliminar(int idAlumno, int idEscuela);

    // Asociación de tutores
    Task<bool> ExisteTutorEnEscuela(int idTutor, int idEscuela);
    Task<AlumnoTutor?> ObtenerAsociacionTutor(int idAlumno, int idTutor);
    Task AsociarTutor(AlumnoTutor alumnoTutor);
    Task<bool> QuitarTutor(int idAlumno, int idTutor);

    Task<bool> GuardarCambios();
    Task<IQueryable<Alumno>> ObtenerQueryable(int idEscuela, string? nombre = null, string? apellido = null, int? dni = null);
}
