using EducAR.API.DTOs.Evaluaciones;

namespace EducAR.API.Services.Interfaces;

public interface IEvaluacionService
{
    Task<ResultadoEvaluacion> Opciones(AccesoEvaluacion acceso);
    Task<ResultadoEvaluacion> Obtener(int idCurso, int idMateria, int idPeriodo, AccesoEvaluacion acceso);
    Task<ResultadoEvaluacion> Crear(EvaluacionCrearDto dto, AccesoEvaluacion acceso);
    Task<ResultadoEvaluacion> Editar(int id, EvaluacionEditarDto dto, AccesoEvaluacion acceso);
    Task<ResultadoEvaluacion> Archivar(int id, AccesoEvaluacion acceso);
    Task<ResultadoEvaluacion> ObtenerAlumnos(int id, AccesoEvaluacion acceso);
    Task<ResultadoEvaluacion> GuardarNotas(int id, NotasEvaluacionGuardarDto dto, AccesoEvaluacion acceso);
}
