using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IPeriodoEvaluacionRepository
{
    Task<List<PeriodoEvaluacion>> ObtenerTodos(int idCicloLectivo);
    Task<PeriodoEvaluacion?> ObtenerPorId(int idPeriodo, int idCicloLectivo);
    Task<bool> ExisteNombre(string nombre, int idCicloLectivo, int? excluirId = null);
    Task<bool> TieneCalificaciones(int idPeriodo);
    Task<PeriodoEvaluacion> Crear(PeriodoEvaluacion periodo);
    Task<bool> Actualizar(PeriodoEvaluacion periodo);
    Task<(bool exito, string mensaje)> Eliminar(int idPeriodo, int idCicloLectivo);
}