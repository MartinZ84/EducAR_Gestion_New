using EducAR.API.DTOs.Matriculas;
using EducAR.API.DTOs.Paginacion;

namespace EducAR.API.Services.Interfaces;

public interface IMatriculaService
{
    Task<List<MatriculaResponseDto>> ObtenerPorCurso(int idCurso, int idEscuela);
    Task<List<MatriculaResponseDto>> ObtenerPorAlumno(int idAlumno, int idEscuela);
    Task<MatriculaResponseDto?> ObtenerPorId(int idMatricula, int idEscuela);
    Task<(bool exito, string mensaje, MatriculaResponseDto? matricula)> Crear(MatriculaCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> DarDeBaja(int idMatricula, int idEscuela);
    Task<ResultadoPaginadoDto<MatriculaAlumnoDisponibleDto>> ObtenerAlumnosParaMatricular(int idEscuela, int anioRegistro, int idCicloLectivo, PaginacionDto paginacion, string? nombre, string? apellido, int? dni);
    Task<(bool exito, string mensaje, MatriculaAsignacionResultadoDto resultado)> AsignarMasivo(MatriculaAsignacionMasivaDto dto, int idEscuela);
}
