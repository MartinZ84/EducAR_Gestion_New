using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class CursoRepository : ICursoRepository
{
    private readonly AppDbContext _context;

    public CursoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Curso>> ObtenerTodos(int idEscuela)
    {
        return await _context.Cursos
            .Include(c => c.CicloLectivo)
            .Include(c => c.Matriculas)
            .Where(c => c.IdEscuela == idEscuela)
            .OrderBy(c => c.CicloLectivo.Anio)
            .ThenBy(c => c.Grado)
            .ThenBy(c => c.Division)
            .ToListAsync();
    }

    public async Task<List<Curso>> ObtenerPorCicloLectivo(int idCicloLectivo, int idEscuela)
    {
        return await _context.Cursos
            .Include(c => c.CicloLectivo)
            .Include(c => c.Matriculas)
            .Where(c => c.IdCicloLectivo == idCicloLectivo && c.IdEscuela == idEscuela)
            .OrderBy(c => c.Grado)
            .ThenBy(c => c.Division)
            .ToListAsync();
    }

    public async Task<Curso?> ObtenerPorId(int idCurso, int idEscuela)
    {
        return await _context.Cursos
            .Include(c => c.CicloLectivo)
            .Include(c => c.Matriculas)
            .FirstOrDefaultAsync(c => c.IdCurso == idCurso && c.IdEscuela == idEscuela);
    }

    public async Task<bool> ExisteCurso(int grado, string division, string? turno, int idCicloLectivo, int? excluirId = null)
    {
        return await _context.Cursos
            .AnyAsync(c => c.Grado == grado &&
                           c.Division == division &&
                           c.Turno == turno &&
                           c.IdCicloLectivo == idCicloLectivo &&
                           (excluirId == null || c.IdCurso != excluirId));
    }

    public async Task<bool> TieneAlumnosInscriptos(int idCurso)
    {
        return await _context.Matriculas
            .AnyAsync(m => m.IdCurso == idCurso && m.Estado == EstadoMatricula.Activa);
    }

    public async Task<bool> TieneAsistencias(int idCurso)
    {
        return await _context.Asistencias
            .AnyAsync(a => a.IdCurso == idCurso);
    }

    public async Task<bool> TieneCalificaciones(int idCurso)
    {
        return await _context.Boletines
            .AnyAsync(b => b.IdCurso == idCurso);
    }

    public async Task<Curso> Crear(Curso curso)
    {
        _context.Cursos.Add(curso);
        await _context.SaveChangesAsync();
        return curso;
    }

    public async Task<bool> Actualizar(Curso curso)
    {
        curso.FechaAct = DateTime.Now;
        _context.Cursos.Update(curso);
        var filas = await _context.SaveChangesAsync();
        return filas > 0;
    }

    public async Task<bool> Eliminar(int idCurso, int idEscuela)
    {
        var curso = await ObtenerPorId(idCurso, idEscuela);
        if (curso is null) return false;

        curso.Activo = false;
        curso.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }

    public Task<IQueryable<Curso>> ObtenerQueryable(int idEscuela, int? grado = null, string? division = null, string? turno = null, int? idCicloLectivo = null)
    {
        var query = _context.Cursos
            .Include(c => c.CicloLectivo)
            .Include(c => c.Matriculas)
            .Where(c => c.IdEscuela == idEscuela)
            .AsQueryable();

        if (grado.HasValue)
            query = query.Where(c => c.Grado == grado.Value);

        if (!string.IsNullOrWhiteSpace(division))
            query = query.Where(c => c.Division.Contains(division));

        if (!string.IsNullOrWhiteSpace(turno))
            query = query.Where(c => c.Turno != null && c.Turno.Contains(turno));

        if (idCicloLectivo.HasValue)
            query = query.Where(c => c.IdCicloLectivo == idCicloLectivo.Value);

        return Task.FromResult(query
            .OrderBy(c => c.CicloLectivo.Anio)
            .ThenBy(c => c.Grado)
            .ThenBy(c => c.Division).AsQueryable());
    }
}
