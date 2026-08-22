using EducAR.API.DTOs.Cursos;
using EducAR.API.DTOs.Paginacion;

namespace EducAR.API.Services.Interfaces;

public interface ICursoService
{
    Task<List<CursoResponseDto>> ObtenerTodos(int idEscuela);
    Task<List<CursoResponseDto>> ObtenerPorCicloLectivo(int idCicloLectivo, int idEscuela);
    Task<CursoResponseDto?> ObtenerPorId(int idCurso, int idEscuela);
    Task<CursoDetalleDto?> ObtenerDetalle(int idCurso, int idEscuela);
    Task<(bool exito, string mensaje, CursoResponseDto? curso)> Crear(CursoCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Actualizar(int idCurso, int idEscuela, CursoUpdateDto dto);
    Task<(bool exito, string mensaje)> Eliminar(int idCurso, int idEscuela);
    Task<ResultadoPaginadoDto<CursoResponseDto>> ObtenerTodosPaginado(int idEscuela, FiltroCursoDto filtro);
}
