using EducAR.API.DTOs.DocenteMateriaCurso;

namespace EducAR.API.Services.Interfaces;

public interface IDocenteMateriaCursoService
{
    Task<List<DocenteMateriaCursoResponseDto>> ObtenerPorDocente(int idDocente, int idEscuela);
    Task<List<DocenteMateriaCursoResponseDto>> ObtenerPorCurso(int idCurso, int idEscuela);
    Task<List<DocenteMateriaCursoResponseDto>> ObtenerPorCursoYMateria(int idCurso, int idMateria, int idEscuela);
    Task<(bool exito, string mensaje, DocenteMateriaCursoResponseDto? asignacion)> Asignar(DocenteMateriaCursoCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Desasignar(int idDocenteMateriaCurso, int idEscuela);
    Task<List<DocenteMateriaCursoResponseDto>> ObtenerMisCursos(int idUsuario, int idEscuela);
}