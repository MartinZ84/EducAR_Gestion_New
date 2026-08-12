using EducAR.API.DTOs.PeriodosEvaluacion;

namespace EducAR.API.Services.Interfaces;

public interface IPeriodoEvaluacionService
{
    Task<List<PeriodoEvaluacionResponseDto>> ObtenerTodos(int idCicloLectivo, int idEscuela);
    Task<PeriodoEvaluacionResponseDto?> ObtenerPorId(int idPeriodo, int idCicloLectivo, int idEscuela);
    Task<(bool exito, string mensaje, PeriodoEvaluacionResponseDto? periodo)> Crear(PeriodoEvaluacionCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Actualizar(int idPeriodo, int idCicloLectivo, int idEscuela, PeriodoEvaluacionUpdateDto dto);
    Task<(bool exito, string mensaje)> Eliminar(int idPeriodo, int idCicloLectivo, int idEscuela);
}