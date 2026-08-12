using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IEscuelaRepository
{
    Task<List<Escuela>> ObtenerTodas();
    Task<Escuela?> ObtenerPorId(int idEscuela);
    Task<bool> ExisteNombre(string nombre, int? excluirId = null);
    Task<Escuela> Crear(Escuela escuela);
    Task<bool> Actualizar(Escuela escuela);
    Task<IQueryable<Escuela>> ObtenerQueryable(string? nombre = null);
}
