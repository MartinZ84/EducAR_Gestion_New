using EducAR.API.Models;

namespace EducAR.API.Repositories.Interfaces;

public interface IMatriculaRepository
{
    Task<List<Matricula>> ObtenerPorCurso(int idCurso, int idEscuela);
    Task<List<Matricula>> ObtenerPorAlumno(int idAlumno, int idEscuela);
    Task<Matricula?> ObtenerPorId(int idMatricula, int idEscuela);
    Task<Matricula?> ObtenerPorAlumnoYCiclo(int idAlumno, int idCicloLectivo, int idEscuela);
    Task<bool> ExisteMatriculaActivaEnCurso(int idAlumno, int idCurso, int idEscuela);
    Task<Matricula> Crear(Matricula matricula);
    Task<bool> Actualizar(Matricula matricula);
    Task<bool> DarDeBaja(int idMatricula, int idEscuela);
    Task<IQueryable<Alumno>> ObtenerAlumnosParaMatricular(int idEscuela, int anioRegistro, int idCicloLectivo);
    Task<List<Matricula>> ObtenerMatriculasPorAlumnosYCiclo(IEnumerable<int> idsAlumnos, int idCicloLectivo, int idEscuela);
    Task<MatriculaAsignacionPersistenciaResultado> AsignarMasivo(int idCurso, int idCicloLectivo, int idEscuela, IReadOnlyCollection<int> idsAlumnos);
}

public sealed class MatriculaAsignacionPersistenciaResultado
{
    public List<Matricula> MatriculasCreadas { get; } = new();
    public List<Matricula> MatriculasReactivadas { get; } = new();
    public List<Matricula> MatriculasYaExistentes { get; } = new();
    public List<MatriculaConflicto> Conflictos { get; } = new();
    public List<int> AlumnosNoEncontrados { get; } = new();
}

public sealed class MatriculaConflicto
{
    public int IdAlumno { get; init; }
    public int? IdCursoActual { get; init; }
    public string? CursoActual { get; init; }
}
