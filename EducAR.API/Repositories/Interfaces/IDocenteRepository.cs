using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IDocenteRepository
{
    Task<List<Docente>> ObtenerTodos(int idEscuela);
    Task<Docente?> ObtenerPorId(int idDocente, int idEscuela);
    Task<Docente?> ObtenerDetalle(int idDocente, int idEscuela);
    Task<Docente> Crear(Docente docente);
    Task<bool> Actualizar(Docente docente);
    Task<bool> Eliminar(int idDocente, int idEscuela);
    Task<Docente?> ObtenerPorUsuarioInactivo(int idUsuario);
    Task<Docente> Reactivar(Docente docente);
    Task<IQueryable<Docente>> ObtenerQueryable(int idEscuela, string? nombre = null, string? apellido = null, int? dni = null);
}
