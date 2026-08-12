using EducAR.API.DTOs.AlumnoTutor;

namespace EducAR.API.Services.Interfaces;

public interface IAlumnoTutorService
{
    Task<List<AlumnoTutorResponseDto>> ObtenerPorAlumno(int idAlumno, int idEscuela);
    Task<List<AlumnoTutorResponseDto>> ObtenerPorTutor(int idTutor, int idEscuela);
    Task<(bool exito, string mensaje, AlumnoTutorResponseDto? relacion)> Asociar(AlumnoTutorCreateDto dto, int idEscuela);
    Task<(bool exito, string mensaje)> Desasociar(int idAlumnoTutor, int idEscuela);
    Task<List<AlumnoTutorResponseDto>> ObtenerMisAlumnos(int idUsuario, int idEscuela);
    
}