using EducAR.API.DTOs.CiclosLectivos;

namespace EducAR.API.Services.Interfaces;

public interface ICicloLectivoService
{
    Task<List<CicloLectivoResponseDto>> ObtenerTodos(int idEscuela);
    Task<CicloLectivoResponseDto?> ObtenerPorId(int idCicloLectivo, int idEscuela);
    Task<(bool exito, string mensaje, CicloLectivoResponseDto? ciclo)> Crear(CicloLectivoCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Actualizar(int idCicloLectivo, int idEscuela, CicloLectivoUpdateDto dto);
    Task<bool> Eliminar(int idCicloLectivo, int idEscuela);
}