using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IMateriaRepository
{
    Task<List<Materia>> ObtenerTodas(int idEscuela);
    Task<Materia?> ObtenerPorId(int idMateria, int idEscuela);
    Task<Materia?> ObtenerDetalle(int idMateria, int idEscuela);
    Task<bool> ExisteNombre(string nombre, int idEscuela, int? excluirId = null);
    Task<Materia> Crear(Materia materia);
    Task<bool> Actualizar(Materia materia);
    Task<bool> Eliminar(int idMateria, int idEscuela);
    Task<IQueryable<Materia>> ObtenerQueryable(int idEscuela, string? nombre = null);
}
