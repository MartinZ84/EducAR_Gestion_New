using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IBoletinRepository
{
    Task<List<Boletin>> ObtenerPorCursoYPeriodo(int idCurso, int idPeriodo);
    Task<Boletin?> ObtenerPorAlumnoCursoYPeriodo(int idAlumno, int idCurso, int idPeriodo);
    Task<Boletin?> ObtenerPorId(int idBoletin);
    Task<Boletin> Crear(Boletin boletin);
    Task<bool> Actualizar(Boletin boletin);
}