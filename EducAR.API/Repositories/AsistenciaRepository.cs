using EducAR.API.Data;
using EducAR.API.Models;
using EducAR.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EducAR.API.Repositories;

public class AsistenciaRepository : IAsistenciaRepository
{
    private readonly AppDbContext _context;

    public AsistenciaRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Asistencia>> ObtenerPorCursoYFecha(int idCurso, DateTime fecha)
    {
        return await _context.Asistencias
            .Include(a => a.Alumno)
            .Where(a => a.IdCurso == idCurso && a.Fecha.Date == fecha.Date)
            .OrderBy(a => a.Alumno.Apellido)
            .ToListAsync();
    }

    public async Task<List<Asistencia>> ObtenerPorAlumnoYCurso(int idAlumno, int idCurso)
    {
        return await _context.Asistencias
            .Include(a => a.Alumno)
            .Where(a => a.IdAlumno == idAlumno && a.IdCurso == idCurso)
            .OrderByDescending(a => a.Fecha)
            .ToListAsync();
    }

    public async Task<bool> ExisteAsistenciaDelDia(int idCurso, DateTime fecha)
    {
        return await _context.Asistencias
            .AnyAsync(a => a.IdCurso == idCurso && a.Fecha.Date == fecha.Date);
    }

    public async Task RegistrarLote(List<Asistencia> asistencias)
    {
        _context.Asistencias.AddRange(asistencias);
        await _context.SaveChangesAsync();
    }

    public async Task ActualizarLote(List<Asistencia> asistencias)
    {
        _context.Asistencias.UpdateRange(asistencias);
        await _context.SaveChangesAsync();
    }
}