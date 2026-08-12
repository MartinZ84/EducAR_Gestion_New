using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface ICicloLectivoRepository
{
    Task<List<CicloLectivo>> ObtenerTodos(int idEscuela);
    Task<CicloLectivo?> ObtenerPorId(int idCicloLectivo, int idEscuela);
    Task<bool> ExisteAnio(int anio, int idEscuela, int? excluirId = null);
    Task<CicloLectivo> Crear(CicloLectivo cicloLectivo);
    Task<bool> Actualizar(CicloLectivo cicloLectivo);
    Task<bool> Eliminar(int idCicloLectivo, int idEscuela);
}