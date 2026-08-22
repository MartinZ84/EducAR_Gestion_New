using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface ITutorRepository
{
    Task<List<Tutor>> ObtenerTodos(int idEscuela);
    Task<Tutor?> ObtenerPorId(int idTutor, int idEscuela);
    Task<Tutor?> ObtenerDetalle(int idTutor, int idEscuela);
    Task<Tutor> Crear(Tutor tutor);
    Task<bool> Actualizar(Tutor tutor);
    Task<bool> Eliminar(int idTutor, int idEscuela);
    Task<Tutor?> ObtenerPorUsuarioInactivo(int idUsuario);
    Task<Tutor> Reactivar(Tutor tutor);
    Task<IQueryable<Tutor>> ObtenerQueryable(int idEscuela, string? nombre = null, string? apellido = null, int? dni = null);
}
