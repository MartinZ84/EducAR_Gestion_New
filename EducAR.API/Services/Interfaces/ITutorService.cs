using EducAR.API.DTOs.Tutores;
using EducAR.API.DTOs.Paginacion;

namespace EducAR.API.Services.Interfaces;

public interface ITutorService
{
    Task<List<TutorResponseDto>> ObtenerTodos(int idEscuela);
    Task<TutorResponseDto?> ObtenerPorId(int idTutor, int idEscuela);
    Task<(bool exito, string mensaje, TutorResponseDto? tutor)> Crear(TutorCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Actualizar(int idTutor, int idEscuela, TutorUpdateDto dto);
    Task<bool> Eliminar(int idTutor, int idEscuela);
    Task<ResultadoPaginadoDto<TutorResponseDto>> ObtenerTodosPaginado(int idEscuela, FiltroPersonaDto filtro);
}
