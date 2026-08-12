using EducAR.API.DTOs.Materias;
using EducAR.API.DTOs.Paginacion;

namespace EducAR.API.Services.Interfaces;

public interface IMateriaService
{
    Task<List<MateriaResponseDto>> ObtenerTodas(int idEscuela);
    Task<MateriaResponseDto?> ObtenerPorId(int idMateria, int idEscuela);
    Task<(bool exito, string mensaje, MateriaResponseDto? materia)> Crear(MateriaCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Actualizar(int idMateria, int idEscuela, MateriaUpdateDto dto);
    Task<bool> Eliminar(int idMateria, int idEscuela);
    Task<ResultadoPaginadoDto<MateriaResponseDto>> ObtenerTodasPaginado(int idEscuela, FiltroNombreDto filtro);
}
