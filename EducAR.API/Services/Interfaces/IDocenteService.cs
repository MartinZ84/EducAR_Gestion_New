using EducAR.API.DTOs.Docentes;
using EducAR.API.DTOs.Paginacion;

namespace EducAR.API.Services.Interfaces;

public interface IDocenteService
{
    Task<List<DocenteResponseDto>> ObtenerTodos(int idEscuela);
    Task<DocenteResponseDto?> ObtenerPorId(int idDocente, int idEscuela);
    Task<DocenteDetalleDto?> ObtenerDetalle(int idDocente, int idEscuela);
    Task<(bool exito, string mensaje, DocenteResponseDto? docente)> Crear(DocenteCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Actualizar(int idDocente, int idEscuela, DocenteUpdateDto dto);
    Task<bool> Eliminar(int idDocente, int idEscuela);
    Task<ResultadoPaginadoDto<DocenteResponseDto>> ObtenerTodosPaginado(int idEscuela, FiltroPersonaDto filtro);
}
