using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class DocenteMateriaCursoRepository : IDocenteMateriaCursoRepository
{
    private readonly AppDbContext _context;

    public DocenteMateriaCursoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DocenteMateriaCurso>> ObtenerPorDocente(int idDocente)
    {
        return await _context.DocenteMateriaCursos
            .Include(dmc => dmc.Docente).ThenInclude(d => d.Usuario)
            .Include(dmc => dmc.Materia)
            .Include(dmc => dmc.Curso).ThenInclude(c => c.CicloLectivo)
            .Where(dmc => dmc.IdDocente == idDocente && dmc.Activo)
            .OrderBy(dmc => dmc.Curso.Grado)
            .ThenBy(dmc => dmc.Materia.Nombre)
            .ToListAsync();
    }

    public async Task<List<DocenteMateriaCurso>> ObtenerPorCurso(int idCurso)
    {
        return await _context.DocenteMateriaCursos
            .Include(dmc => dmc.Docente).ThenInclude(d => d.Usuario)
            .Include(dmc => dmc.Materia)
            .Include(dmc => dmc.Curso).ThenInclude(c => c.CicloLectivo)
            .Where(dmc => dmc.IdCurso == idCurso && dmc.Activo)
            .OrderBy(dmc => dmc.Materia.Nombre)
            .ToListAsync();
    }

    public async Task<List<DocenteMateriaCurso>> ObtenerPorCursoYMateria(int idCurso, int idMateria)
    {
        return await _context.DocenteMateriaCursos
            .Include(dmc => dmc.Docente).ThenInclude(d => d.Usuario)
            .Include(dmc => dmc.Materia)
            .Include(dmc => dmc.Curso).ThenInclude(c => c.CicloLectivo)
            .Where(dmc => dmc.IdCurso == idCurso &&
                          dmc.IdMateria == idMateria &&
                          dmc.Activo)
            .ToListAsync();
    }

    public async Task<DocenteMateriaCurso?> ObtenerPorId(int idDocenteMateriaCurso)
    {
        return await _context.DocenteMateriaCursos
            .Include(dmc => dmc.Docente).ThenInclude(d => d.Usuario)
            .Include(dmc => dmc.Materia)
            .Include(dmc => dmc.Curso).ThenInclude(c => c.CicloLectivo)
            .FirstOrDefaultAsync(dmc => dmc.IdDocenteMateriaCurso == idDocenteMateriaCurso);
    }

    public async Task<DocenteMateriaCurso?> ObtenerAsignacionActiva(int idDocente, int idMateria, int idCurso)
    {
        return await _context.DocenteMateriaCursos
            .FirstOrDefaultAsync(dmc => dmc.IdDocente == idDocente &&
                                        dmc.IdMateria == idMateria &&
                                        dmc.IdCurso == idCurso &&
                                        dmc.Activo);
    }

    public async Task<DocenteMateriaCurso?> ObtenerAsignacionInactiva(int idDocente, int idMateria, int idCurso)
    {
        return await _context.DocenteMateriaCursos
            .FirstOrDefaultAsync(dmc => dmc.IdDocente == idDocente &&
                                        dmc.IdMateria == idMateria &&
                                        dmc.IdCurso == idCurso &&
                                        !dmc.Activo);
    }

    public async Task<bool> ExisteAsignacion(int idDocente, int idMateria, int idCurso)
    {
        return await _context.DocenteMateriaCursos
            .AnyAsync(dmc => dmc.IdDocente == idDocente &&
                             dmc.IdMateria == idMateria &&
                             dmc.IdCurso == idCurso &&
                             dmc.Activo);
    }

    public async Task<DocenteMateriaCurso> Crear(DocenteMateriaCurso asignacion)
    {
        _context.DocenteMateriaCursos.Add(asignacion);
        await _context.SaveChangesAsync();
        return asignacion;
    }

    public async Task<DocenteMateriaCurso> Reactivar(DocenteMateriaCurso asignacion)
    {
        asignacion.Activo           = true;
        asignacion.FechaAsignacion  = DateTime.Now;
        asignacion.FechaAct         = DateTime.Now;
        _context.DocenteMateriaCursos.Update(asignacion);
        await _context.SaveChangesAsync();
        return asignacion;
    }

    public async Task<bool> Eliminar(int idDocenteMateriaCurso, int idEscuela)
    {
        var asignacion = await ObtenerPorId(idDocenteMateriaCurso);
        if (asignacion is null) return false;

        if (asignacion.Curso.IdEscuela != idEscuela) return false;

        asignacion.Activo   = false;
        asignacion.FechaAct = DateTime.Now;
        await _context.SaveChangesAsync();
        return true;
    }
}