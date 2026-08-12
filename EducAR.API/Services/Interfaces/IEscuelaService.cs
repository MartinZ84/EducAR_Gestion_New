using EducAR.API.DTOs.Escuelas;
using EducAR.API.DTOs.Paginacion;

namespace EducAR.API.Services.Interfaces;

public interface IEscuelaService
{
    Task<List<EscuelaResponseDto>> ObtenerTodas();
    Task<EscuelaResponseDto?> ObtenerPorId(int idEscuela);
    Task<(bool exito, string mensaje, EscuelaResponseDto? escuela)> Crear(EscuelaCreateDto dto);
    Task<(bool exito, string mensaje)> Actualizar(int idEscuela, EscuelaUpdateDto dto);
    Task<ResultadoPaginadoDto<EscuelaResponseDto>> ObtenerTodasPaginado(FiltroNombreDto filtro);
}
