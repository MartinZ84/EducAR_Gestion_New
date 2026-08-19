using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class MatriculaRepository : IMatriculaRepository
{
    private readonly AppDbContext _context;

    public MatriculaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Matricula>> ObtenerPorCurso(int idCurso, int idEscuela)
    {
        return await BaseQuery()
            .Where(m => m.IdCurso == idCurso && m.IdEscuela == idEscuela && m.Estado == EstadoMatricula.Activa)
            .OrderBy(m => m.Alumno.Apellido)
            .ThenBy(m => m.Alumno.Nombre)
            .ThenBy(m => m.Alumno.Dni)
            .ThenBy(m => m.Alumno.FechaNacimiento)
            .ToListAsync();
    }

    public async Task<List<Matricula>> ObtenerPorAlumno(int idAlumno, int idEscuela)
    {
        return await BaseQuery()
            .Where(m => m.IdAlumno == idAlumno && m.IdEscuela == idEscuela)
            .OrderByDescending(m => m.CicloLectivo.Anio)
            .ToListAsync();
    }

    public async Task<Matricula?> ObtenerPorId(int idMatricula, int idEscuela)
    {
        return await BaseQuery()
            .FirstOrDefaultAsync(m => m.IdMatricula == idMatricula && m.IdEscuela == idEscuela);
    }

    public async Task<Matricula?> ObtenerPorAlumnoYCiclo(int idAlumno, int idCicloLectivo, int idEscuela)
    {
        return await BaseQuery()
            .FirstOrDefaultAsync(m => m.IdAlumno == idAlumno &&
                                      m.IdCicloLectivo == idCicloLectivo &&
                                      m.IdEscuela == idEscuela);
    }

    public async Task<bool> ExisteMatriculaActivaEnCurso(int idAlumno, int idCurso, int idEscuela)
    {
        return await _context.Matriculas.AnyAsync(m =>
            m.IdAlumno == idAlumno &&
            m.IdCurso == idCurso &&
            m.IdEscuela == idEscuela &&
            m.Estado == EstadoMatricula.Activa);
    }

    public async Task<Matricula> Crear(Matricula matricula)
    {
        _context.Matriculas.Add(matricula);
        await _context.SaveChangesAsync();
        return matricula;
    }

    public async Task<bool> Actualizar(Matricula matricula)
    {
        matricula.FechaAct = DateTime.Now;
        _context.Matriculas.Update(matricula);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DarDeBaja(int idMatricula, int idEscuela)
    {
        var matricula = await _context.Matriculas
            .FirstOrDefaultAsync(m => m.IdMatricula == idMatricula && m.IdEscuela == idEscuela);

        if (matricula is null || matricula.Estado != EstadoMatricula.Activa)
            return false;

        matricula.Estado = EstadoMatricula.Baja;
        matricula.FechaBaja = DateTime.Now;
        matricula.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<IQueryable<Alumno>> ObtenerAlumnosParaMatricular(int idEscuela, int anioRegistro, int idCicloLectivo)
    {
        var inicio = new DateTime(anioRegistro, 1, 1);
        var fin = inicio.AddYears(1);

        var query = _context.Alumnos
            .Where(a => a.IdEscuela == idEscuela &&
                        a.Activo &&
                        a.FechaCrea >= inicio &&
                        a.FechaCrea < fin)
            .Select(a => a);

        return Task.FromResult(query);
    }

    public async Task<List<Matricula>> ObtenerMatriculasPorAlumnosYCiclo(IEnumerable<int> idsAlumnos, int idCicloLectivo, int idEscuela)
    {
        var ids = idsAlumnos.Distinct().ToList();
        if (ids.Count == 0) return new List<Matricula>();

        return await BaseQuery()
            .Where(m => ids.Contains(m.IdAlumno) &&
                        m.IdCicloLectivo == idCicloLectivo &&
                        m.IdEscuela == idEscuela)
            .ToListAsync();
    }

    public async Task<MatriculaAsignacionPersistenciaResultado> AsignarMasivo(
        int idCurso, int idCicloLectivo, int idEscuela, IReadOnlyCollection<int> idsAlumnos)
    {
        var resultado = new MatriculaAsignacionPersistenciaResultado();
        var ids = idsAlumnos.Distinct().ToList();

        await using var transaction = await _context.Database.BeginTransactionAsync();

        var matriculas = await BaseQuery()
            .Where(m => ids.Contains(m.IdAlumno) &&
                        m.IdCicloLectivo == idCicloLectivo &&
                        m.IdEscuela == idEscuela)
            .ToListAsync();

        var alumnos = await _context.Alumnos
            .Where(a => ids.Contains(a.IdAlumno) && a.IdEscuela == idEscuela && a.Activo)
            .ToDictionaryAsync(a => a.IdAlumno);

        foreach (var idAlumno in ids)
        {
            if (!alumnos.TryGetValue(idAlumno, out var alumno))
            {
                resultado.AlumnosNoEncontrados.Add(idAlumno);
                continue;
            }

            var existente = matriculas.FirstOrDefault(m => m.IdAlumno == idAlumno);

            if (existente is null)
            {
                var nueva = new Matricula
                {
                    IdEscuela = idEscuela,
                    IdAlumno = idAlumno,
                    IdCurso = idCurso,
                    IdCicloLectivo = idCicloLectivo,
                    FechaMatricula = DateTime.Now,
                    Estado = EstadoMatricula.Activa,
                    Alumno = alumno
                };

                _context.Matriculas.Add(nueva);
                resultado.MatriculasCreadas.Add(nueva);
                continue;
            }

            if (existente.Estado == EstadoMatricula.Activa && existente.IdCurso == idCurso)
            {
                resultado.MatriculasYaExistentes.Add(existente);
                continue;
            }

            if (existente.Estado == EstadoMatricula.Activa && existente.IdCurso != idCurso)
            {
                resultado.Conflictos.Add(new MatriculaConflicto
                {
                    IdAlumno = idAlumno,
                    IdCursoActual = existente.IdCurso,
                    CursoActual = $"{existente.Curso.Grado}° {existente.Curso.Division} - {existente.CicloLectivo.Anio}"
                });
                continue;
            }

            existente.IdCurso = idCurso;
            existente.Estado = EstadoMatricula.Activa;
            existente.FechaBaja = null;
            existente.FechaMatricula = DateTime.Now;
            existente.FechaAct = DateTime.Now;
            resultado.MatriculasReactivadas.Add(existente);
        }

        await _context.SaveChangesAsync();
        await transaction.CommitAsync();
        return resultado;
    }

    private IQueryable<Matricula> BaseQuery()
    {
        return _context.Matriculas
            .Include(m => m.Alumno)
            .Include(m => m.Curso)
                .ThenInclude(c => c.CicloLectivo);
    }
}
