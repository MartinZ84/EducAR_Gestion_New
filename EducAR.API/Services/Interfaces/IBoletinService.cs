using EducAR.API.DTOs.Boletines;

namespace EducAR.API.Services.Interfaces;

public interface IBoletinService
{
    Task<List<BoletinResponseDto>> ObtenerPorCursoYPeriodo(int idCurso, int idPeriodo, int idEscuela);
    Task<BoletinResponseDto?> ObtenerPorAlumno(int idAlumno, int idCurso, int idPeriodo, int idEscuela);
    Task<(bool exito, string mensaje, int boletinesGenerados)> Generar(BoletinGenerarDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> ActualizarObservacion(int idBoletin, BoletinObservacionDto dto, int idEscuela);
}